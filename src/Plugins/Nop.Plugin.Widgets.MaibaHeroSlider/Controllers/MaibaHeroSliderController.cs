using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Widgets.MaibaHeroSlider.Domain;
using Nop.Plugin.Widgets.MaibaHeroSlider.Models;
using Nop.Plugin.Widgets.MaibaHeroSlider.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Widgets.MaibaHeroSlider.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
public class MaibaHeroSliderController : BasePluginController
{
    #region Fields

    private readonly IHeroSlideService _heroSlideService;
    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;
    private readonly IPermissionService _permissionService;
    private readonly ISettingService _settingService;

    #endregion

    #region Ctor

    public MaibaHeroSliderController(
        IHeroSlideService heroSlideService,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IPermissionService permissionService,
        ISettingService settingService)
    {
        _heroSlideService = heroSlideService;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _permissionService = permissionService;
        _settingService = settingService;
    }

    #endregion

    #region Methods

    public async Task<IActionResult> Configure()
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermission.Configuration.MANAGE_WIDGETS))
            return AccessDeniedView();

        var model = new ConfigurationModel();

        // Load all slides for the list
        var slides = await _heroSlideService.GetAllSlidesAsync();
        foreach (var slide in slides)
        {
            model.Slides.Add(new HeroSlideListModel
            {
                Id = slide.Id,
                DisplayOrder = slide.DisplayOrder,
                Heading = slide.Heading,
                IsActive = slide.IsActive
            });
        }

        return View("~/Themes/Maiba/Plugins/Nop.Plugin.Widgets.MaibaHeroSlider/Views/Configure.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermission.Configuration.MANAGE_WIDGETS))
            return AccessDeniedView();

        if (!ModelState.IsValid)
            return await Configure();

        // Create new slide
        var slide = new HeroSlide
        {
            DisplayOrder = model.DisplayOrder,
            PictureId = model.PictureId,
            ThumbnailPictureId = model.ThumbnailPictureId,
            Heading = model.Heading ?? string.Empty,
            Tag = model.Tag ?? string.Empty,
            Description = model.Description ?? string.Empty,
            ButtonText = model.ButtonText ?? string.Empty,
            ButtonUrl = model.ButtonUrl ?? string.Empty,
            IsActive = model.IsActive
        };

        await _heroSlideService.InsertSlideAsync(slide);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return await Configure();
    }

    [HttpPost]
    public async Task<IActionResult> SlideDelete(int id)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermission.Configuration.MANAGE_WIDGETS))
            return AccessDeniedView();

        var slide = await _heroSlideService.GetSlideByIdAsync(id);
        if (slide == null)
            return Json(new { Result = false });

        await _heroSlideService.DeleteSlideAsync(slide);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Common.ItemDeleted"));

        return Json(new { Result = true });
    }

    public async Task<IActionResult> SlideEdit(int id)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermission.Configuration.MANAGE_WIDGETS))
            return AccessDeniedView();

        var slide = await _heroSlideService.GetSlideByIdAsync(id);
        if (slide == null)
            return RedirectToAction("Configure");

        var model = new ConfigurationModel
        {
            DisplayOrder = slide.DisplayOrder,
            PictureId = slide.PictureId,
            ThumbnailPictureId = slide.ThumbnailPictureId,
            Heading = slide.Heading,
            Tag = slide.Tag,
            Description = slide.Description,
            ButtonText = slide.ButtonText,
            ButtonUrl = slide.ButtonUrl,
            IsActive = slide.IsActive
        };

        return View("~/Themes/Maiba/Plugins/Nop.Plugin.Widgets.MaibaHeroSlider/Views/SlideEdit.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> SlideEdit(int id, ConfigurationModel model)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermission.Configuration.MANAGE_WIDGETS))
            return AccessDeniedView();

        var slide = await _heroSlideService.GetSlideByIdAsync(id);
        if (slide == null)
            return RedirectToAction("Configure");

        if (!ModelState.IsValid)
            return View("~/Themes/Maiba/Plugins/Nop.Plugin.Widgets.MaibaHeroSlider/Views/SlideEdit.cshtml", model);

        slide.DisplayOrder = model.DisplayOrder;
        slide.PictureId = model.PictureId;
        slide.ThumbnailPictureId = model.ThumbnailPictureId;
        slide.Heading = model.Heading ?? string.Empty;
        slide.Tag = model.Tag ?? string.Empty;
        slide.Description = model.Description ?? string.Empty;
        slide.ButtonText = model.ButtonText ?? string.Empty;
        slide.ButtonUrl = model.ButtonUrl ?? string.Empty;
        slide.IsActive = model.IsActive;

        await _heroSlideService.UpdateSlideAsync(slide);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return RedirectToAction("Configure");
    }

    #endregion
}

