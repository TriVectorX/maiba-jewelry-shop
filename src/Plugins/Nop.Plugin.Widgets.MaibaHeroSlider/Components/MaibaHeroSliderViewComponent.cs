using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Widgets.MaibaHeroSlider.Models;
using Nop.Plugin.Widgets.MaibaHeroSlider.Services;
using Nop.Services.Media;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.MaibaHeroSlider.Components;

/// <summary>
/// Maiba Hero Slider view component
/// </summary>
public partial class MaibaHeroSliderViewComponent : NopViewComponent
{
    #region Fields

    private readonly IHeroSlideService _heroSlideService;
    private readonly IPictureService _pictureService;

    #endregion

    #region Ctor

    public MaibaHeroSliderViewComponent(
        IHeroSlideService heroSlideService,
        IPictureService pictureService)
    {
        _heroSlideService = heroSlideService;
        _pictureService = pictureService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <param name="widgetZone">Widget zone name</param>
    /// <param name="additionalData">Additional data</param>
    /// <returns>View component result</returns>
    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
        var slides = await _heroSlideService.GetAllSlidesAsync(activeOnly: true);

        var model = new PublicInfoModel();

        foreach (var slide in slides)
        {
            var pictureUrl = await _pictureService.GetPictureUrlAsync(slide.PictureId, 1400);
            
            // If thumbnail is not set, auto-generate from main image
            var thumbnailUrl = slide.ThumbnailPictureId > 0
                ? await _pictureService.GetPictureUrlAsync(slide.ThumbnailPictureId, 150)
                : await _pictureService.GetPictureUrlAsync(slide.PictureId, 150);

            model.Slides.Add(new HeroSlideModel
            {
                Id = slide.Id,
                DisplayOrder = slide.DisplayOrder,
                PictureUrl = pictureUrl,
                ThumbnailUrl = thumbnailUrl,
                Heading = slide.Heading,
                Tag = slide.Tag,
                Description = slide.Description,
                ButtonText = slide.ButtonText,
                ButtonUrl = slide.ButtonUrl
            });
        }

        if (!model.Slides.Any())
            return Content("");

        return View("~/Themes/Maiba/Plugins/Nop.Plugin.Widgets.MaibaHeroSlider/Views/PublicInfo.cshtml", model);
    }

    #endregion
}

