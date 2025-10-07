using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;

namespace Nop.Plugin.Security.AdminAccess;

public class AdminAccessPlugin : BasePlugin
{
    private readonly ISettingService _settingService;
    private readonly ILocalizationService _localizationService;
    private readonly IWebHelper _webHelper;

    public AdminAccessPlugin(
        ISettingService settingService,
        ILocalizationService localizationService,
        IWebHelper webHelper)
    {
        _settingService = settingService;
        _localizationService = localizationService;
        _webHelper = webHelper;
    }

    public override string GetConfigurationPageUrl()
    {
        return $"{_webHelper.GetStoreLocation()}Admin/AdminAccess/Configure";
    }

    public override async Task InstallAsync()
    {
        var settings = new AdminAccessSettings
        {
            EnableIpWhitelist = true,
            AllowedIpAddresses = "127.0.0.1,::1",
            EnableSecretKey = true,
            SecretAccessKey = GenerateSecretKey(),
            CookieExpirationHours = 24,
            AccessDeniedMessage = "Access to the administration area is restricted. Please contact the site administrator."
        };

        await _settingService.SaveSettingAsync(settings);

        await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Security.AdminAccess.Fields.EnableIpWhitelist", "Enable IP Whitelist");
        await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Security.AdminAccess.Fields.EnableIpWhitelist.Hint", "Enable IP address whitelist restriction");
        await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Security.AdminAccess.Fields.AllowedIpAddresses", "Allowed IP Addresses");
        await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Security.AdminAccess.Fields.AllowedIpAddresses.Hint", "Comma-separated list of allowed IP addresses (e.g., 192.168.1.1,10.0.0.5)");
        await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Security.AdminAccess.Fields.EnableSecretKey", "Enable Secret Key Access");
        await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Security.AdminAccess.Fields.EnableSecretKey.Hint", "Allow access using a secret key parameter (/admin?key=YOUR_KEY)");
        await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Security.AdminAccess.Fields.SecretAccessKey", "Secret Access Key");
        await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Security.AdminAccess.Fields.SecretAccessKey.Hint", "Secret key for one-time authentication. Users can access /admin?key=YOUR_KEY to authenticate.");
        await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Security.AdminAccess.Fields.CookieExpirationHours", "Cookie Expiration (Hours)");
        await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Security.AdminAccess.Fields.CookieExpirationHours.Hint", "How long the authentication cookie remains valid after successful validation");
        await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Security.AdminAccess.Fields.AccessDeniedMessage", "Access Denied Message");
        await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Security.AdminAccess.Fields.AccessDeniedMessage.Hint", "Custom message shown when access is denied");
        await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Security.AdminAccess.Fields.RegenerateKey", "Regenerate Secret Key");

        await base.InstallAsync();
    }

    public override async Task UninstallAsync()
    {
        await _settingService.DeleteSettingAsync<AdminAccessSettings>();

        await _localizationService.DeleteLocaleResourceAsync("Plugins.Security.AdminAccess.Fields.EnableIpWhitelist");
        await _localizationService.DeleteLocaleResourceAsync("Plugins.Security.AdminAccess.Fields.EnableIpWhitelist.Hint");
        await _localizationService.DeleteLocaleResourceAsync("Plugins.Security.AdminAccess.Fields.AllowedIpAddresses");
        await _localizationService.DeleteLocaleResourceAsync("Plugins.Security.AdminAccess.Fields.AllowedIpAddresses.Hint");
        await _localizationService.DeleteLocaleResourceAsync("Plugins.Security.AdminAccess.Fields.EnableSecretKey");
        await _localizationService.DeleteLocaleResourceAsync("Plugins.Security.AdminAccess.Fields.EnableSecretKey.Hint");
        await _localizationService.DeleteLocaleResourceAsync("Plugins.Security.AdminAccess.Fields.SecretAccessKey");
        await _localizationService.DeleteLocaleResourceAsync("Plugins.Security.AdminAccess.Fields.SecretAccessKey.Hint");
        await _localizationService.DeleteLocaleResourceAsync("Plugins.Security.AdminAccess.Fields.CookieExpirationHours");
        await _localizationService.DeleteLocaleResourceAsync("Plugins.Security.AdminAccess.Fields.CookieExpirationHours.Hint");
        await _localizationService.DeleteLocaleResourceAsync("Plugins.Security.AdminAccess.Fields.AccessDeniedMessage");
        await _localizationService.DeleteLocaleResourceAsync("Plugins.Security.AdminAccess.Fields.AccessDeniedMessage.Hint");
        await _localizationService.DeleteLocaleResourceAsync("Plugins.Security.AdminAccess.Fields.RegenerateKey");

        await base.UninstallAsync();
    }

    private static string GenerateSecretKey()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 32)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}

