using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Security.AdminAccess.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Security.AdminAccess.Controllers;

[Area(AreaNames.ADMIN)]
[AuthorizeAdmin]
[AutoValidateAntiforgeryToken]
public class AdminAccessController : BasePluginController
{
    private readonly ISettingService _settingService;
    private readonly IStoreContext _storeContext;
    private readonly INotificationService _notificationService;
    private readonly ILocalizationService _localizationService;

    public AdminAccessController(
        ISettingService settingService,
        IStoreContext storeContext,
        INotificationService notificationService,
        ILocalizationService localizationService)
    {
        _settingService = settingService;
        _storeContext = storeContext;
        _notificationService = notificationService;
        _localizationService = localizationService;
    }

    public async Task<IActionResult> Configure()
    {
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var settings = await _settingService.LoadSettingAsync<AdminAccessSettings>(storeScope);

        var model = new ConfigurationModel
        {
            EnableIpWhitelist = settings.EnableIpWhitelist,
            AllowedIpAddresses = settings.AllowedIpAddresses,
            EnableSecretKey = settings.EnableSecretKey,
            SecretAccessKey = settings.SecretAccessKey,
            CookieExpirationHours = settings.CookieExpirationHours,
            AccessDeniedMessage = settings.AccessDeniedMessage,
            ActiveStoreScopeConfiguration = storeScope
        };

        return View("~/Themes/Maiba/Plugins/Nop.Plugin.Security.AdminAccess/Views/Configure.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var settings = await _settingService.LoadSettingAsync<AdminAccessSettings>(storeScope);

        settings.EnableIpWhitelist = model.EnableIpWhitelist;
        settings.AllowedIpAddresses = model.AllowedIpAddresses;
        settings.EnableSecretKey = model.EnableSecretKey;
        settings.SecretAccessKey = model.SecretAccessKey;
        settings.CookieExpirationHours = model.CookieExpirationHours;
        settings.AccessDeniedMessage = model.AccessDeniedMessage;

        await _settingService.SaveSettingAsync(settings, storeScope);
        await _settingService.ClearCacheAsync();

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return await Configure();
    }

    [HttpPost]
    public async Task<IActionResult> RegenerateKey()
    {
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var settings = await _settingService.LoadSettingAsync<AdminAccessSettings>(storeScope);

        settings.SecretAccessKey = GenerateSecretKey();
        await _settingService.SaveSettingAsync(settings, storeScope);
        await _settingService.ClearCacheAsync();

        _notificationService.SuccessNotification("Secret key regenerated successfully!");

        return await Configure();
    }

    private static string GenerateSecretKey()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 32)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}

