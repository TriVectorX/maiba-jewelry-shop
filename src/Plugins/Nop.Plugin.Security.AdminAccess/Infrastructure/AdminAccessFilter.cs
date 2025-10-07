using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Services.Configuration;
using System.Net;

namespace Nop.Plugin.Security.AdminAccess.Infrastructure;

public class AdminAccessFilter : IAsyncAuthorizationFilter
{
    private readonly ISettingService _settingService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IStoreContext _storeContext;

    private const string ADMIN_ACCESS_COOKIE_NAME = "nop.admin.access.validated";

    public AdminAccessFilter(
        ISettingService settingService,
        IHttpContextAccessor httpContextAccessor,
        IStoreContext storeContext)
    {
        _settingService = settingService;
        _httpContextAccessor = httpContextAccessor;
        _storeContext = storeContext;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        // Don't apply filter to the plugin configuration page itself
        var actionDescriptor = context.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
        if (actionDescriptor != null && 
            actionDescriptor.ControllerName == "AdminAccess" && 
            actionDescriptor.ActionName == "Configure")
        {
            return;
        }

        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var settings = await _settingService.LoadSettingAsync<AdminAccessSettings>(storeScope);

        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            return;

        // Check if already validated via cookie
        if (httpContext.Request.Cookies.TryGetValue(ADMIN_ACCESS_COOKIE_NAME, out var cookieValue) && 
            cookieValue == "true")
        {
            return;
        }

        // Check secret key in query string
        if (settings.EnableSecretKey && !string.IsNullOrEmpty(settings.SecretAccessKey))
        {
            var keyParam = httpContext.Request.Query["key"].ToString();
            if (!string.IsNullOrEmpty(keyParam) && keyParam == settings.SecretAccessKey)
            {
                // Valid secret key - set cookie
                SetAccessCookie(httpContext, settings.CookieExpirationHours);
                return;
            }
        }

        // Check IP whitelist
        if (settings.EnableIpWhitelist && !string.IsNullOrEmpty(settings.AllowedIpAddresses))
        {
            var clientIp = GetClientIpAddress(httpContext);
            var allowedIps = settings.AllowedIpAddresses
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(ip => ip.Trim())
                .ToList();

            if (allowedIps.Any(allowedIp => allowedIp == clientIp || allowedIp == "*"))
            {
                // IP is whitelisted - set cookie
                SetAccessCookie(httpContext, settings.CookieExpirationHours);
                return;
            }
        }

        // Access denied
        context.Result = new ContentResult
        {
            Content = $@"
<!DOCTYPE html>
<html>
<head>
    <title>Access Denied</title>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            display: flex;
            align-items: center;
            justify-content: center;
            min-height: 100vh;
            margin: 0;
            padding: 20px;
        }}
        .container {{
            background: white;
            padding: 40px;
            border-radius: 10px;
            box-shadow: 0 10px 40px rgba(0,0,0,0.2);
            max-width: 500px;
            text-align: center;
        }}
        h1 {{
            color: #e74c3c;
            margin-top: 0;
        }}
        p {{
            color: #666;
            line-height: 1.6;
        }}
        .icon {{
            font-size: 60px;
            margin-bottom: 20px;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='icon'>🔒</div>
        <h1>Access Denied</h1>
        <p>{settings.AccessDeniedMessage}</p>
    </div>
</body>
</html>",
            ContentType = "text/html",
            StatusCode = 403
        };
    }

    private static void SetAccessCookie(HttpContext httpContext, int expirationHours)
    {
        httpContext.Response.Cookies.Append(ADMIN_ACCESS_COOKIE_NAME, "true", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(expirationHours)
        });
    }

    private static string GetClientIpAddress(HttpContext httpContext)
    {
        // Check for forwarded IP (behind proxy)
        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            var ips = forwardedFor.Split(',');
            if (ips.Length > 0)
                return ips[0].Trim();
        }

        // Check for real IP header
        var realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
            return realIp;

        // Get direct connection IP
        return httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}

