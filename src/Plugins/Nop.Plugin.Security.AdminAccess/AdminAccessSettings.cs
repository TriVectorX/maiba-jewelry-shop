using Nop.Core.Configuration;

namespace Nop.Plugin.Security.AdminAccess;

public class AdminAccessSettings : ISettings
{
    /// <summary>
    /// Enable IP whitelist restriction
    /// </summary>
    public bool EnableIpWhitelist { get; set; } = true;

    /// <summary>
    /// Comma-separated list of allowed IP addresses
    /// </summary>
    public string AllowedIpAddresses { get; set; } = "127.0.0.1,::1";

    /// <summary>
    /// Enable secret key access
    /// </summary>
    public bool EnableSecretKey { get; set; } = true;

    /// <summary>
    /// Secret access key for one-time authentication
    /// Format: /admin?key=YOUR_SECRET_KEY
    /// </summary>
    public string SecretAccessKey { get; set; } = string.Empty;

    /// <summary>
    /// Cookie expiration in hours (after validating with secret key or IP)
    /// </summary>
    public int CookieExpirationHours { get; set; } = 24;

    /// <summary>
    /// Custom access denied message
    /// </summary>
    public string AccessDeniedMessage { get; set; } = "Access to the administration area is restricted. Please contact the site administrator.";
}

