using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Security.AdminAccess.Models;

public record ConfigurationModel : BaseNopModel
{
    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Plugins.Security.AdminAccess.Fields.EnableIpWhitelist")]
    public bool EnableIpWhitelist { get; set; }

    [NopResourceDisplayName("Plugins.Security.AdminAccess.Fields.AllowedIpAddresses")]
    public string AllowedIpAddresses { get; set; } = string.Empty;

    [NopResourceDisplayName("Plugins.Security.AdminAccess.Fields.EnableSecretKey")]
    public bool EnableSecretKey { get; set; }

    [NopResourceDisplayName("Plugins.Security.AdminAccess.Fields.SecretAccessKey")]
    public string SecretAccessKey { get; set; } = string.Empty;

    [NopResourceDisplayName("Plugins.Security.AdminAccess.Fields.CookieExpirationHours")]
    public int CookieExpirationHours { get; set; }

    [NopResourceDisplayName("Plugins.Security.AdminAccess.Fields.AccessDeniedMessage")]
    public string AccessDeniedMessage { get; set; } = string.Empty;
}

