using Nop.Core;
using Nop.Plugin.Widgets.MaibaHeroSlider.Components;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Widgets.MaibaHeroSlider;

/// <summary>
/// Maiba Hero Slider plugin
/// </summary>
public class MaibaHeroSliderPlugin : BasePlugin, IWidgetPlugin
{
    #region Fields

    private readonly ILocalizationService _localizationService;
    private readonly INopUrlHelper _nopUrlHelper;
    private readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public MaibaHeroSliderPlugin(
        ILocalizationService localizationService,
        INopUrlHelper nopUrlHelper,
        IWebHelper webHelper)
    {
        _localizationService = localizationService;
        _nopUrlHelper = nopUrlHelper;
        _webHelper = webHelper;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
        return _nopUrlHelper.RouteUrl(MaibaHeroSliderDefaults.ConfigurationRouteName);
    }

    /// <summary>
    /// Gets widget zones where this widget should be rendered
    /// </summary>
    public Task<IList<string>> GetWidgetZonesAsync()
    {
        return Task.FromResult<IList<string>>(new List<string>
        {
            PublicWidgetZones.HomepageTop
        });
    }

    /// <summary>
    /// Gets a type of a view component for displaying widget
    /// </summary>
    /// <param name="widgetZone">Name of the widget zone</param>
    public Type GetWidgetViewComponent(string widgetZone)
    {
        ArgumentNullException.ThrowIfNull(widgetZone);

        return typeof(MaibaHeroSliderViewComponent);
    }

    /// <summary>
    /// Install the plugin
    /// </summary>
    public override async Task InstallAsync()
    {
        // Add localization resources
        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.Widgets.MaibaHeroSlider.Fields.DisplayOrder"] = "Display Order",
            ["Plugins.Widgets.MaibaHeroSlider.Fields.DisplayOrder.Hint"] = "The display order of the slide. 1 represents the first item in the list.",
            ["Plugins.Widgets.MaibaHeroSlider.Fields.PictureId"] = "Picture",
            ["Plugins.Widgets.MaibaHeroSlider.Fields.PictureId.Hint"] = "Upload the main slide image.",
            ["Plugins.Widgets.MaibaHeroSlider.Fields.ThumbnailPictureId"] = "Thumbnail Picture",
            ["Plugins.Widgets.MaibaHeroSlider.Fields.ThumbnailPictureId.Hint"] = "Upload the thumbnail image for vertical navigation.",
            ["Plugins.Widgets.MaibaHeroSlider.Fields.Heading"] = "Heading",
            ["Plugins.Widgets.MaibaHeroSlider.Fields.Heading.Hint"] = "Enter the main heading text for the slide.",
            ["Plugins.Widgets.MaibaHeroSlider.Fields.Tag"] = "Tag",
            ["Plugins.Widgets.MaibaHeroSlider.Fields.Tag.Hint"] = "Enter a tag (e.g., 'GLAMOROUS LIFE').",
            ["Plugins.Widgets.MaibaHeroSlider.Fields.Description"] = "Description",
            ["Plugins.Widgets.MaibaHeroSlider.Fields.Description.Hint"] = "Enter the slide description text.",
            ["Plugins.Widgets.MaibaHeroSlider.Fields.ButtonText"] = "Button Text",
            ["Plugins.Widgets.MaibaHeroSlider.Fields.ButtonText.Hint"] = "Enter the call-to-action button text.",
            ["Plugins.Widgets.MaibaHeroSlider.Fields.ButtonUrl"] = "Button URL",
            ["Plugins.Widgets.MaibaHeroSlider.Fields.ButtonUrl.Hint"] = "Enter the URL for the button link.",
            ["Plugins.Widgets.MaibaHeroSlider.Fields.IsActive"] = "Is Active",
            ["Plugins.Widgets.MaibaHeroSlider.Fields.IsActive.Hint"] = "Check to make this slide active/visible.",
            ["Plugins.Widgets.MaibaHeroSlider.ManageSlides"] = "Manage Slides",
            ["Plugins.Widgets.MaibaHeroSlider.AddNew"] = "Add New Slide",
            ["Plugins.Widgets.MaibaHeroSlider.Edit"] = "Edit Slide",
            ["Plugins.Widgets.MaibaHeroSlider.Delete"] = "Delete Slide",
            ["Plugins.Widgets.MaibaHeroSlider.Slides"] = "Hero Slides",
        });

        await base.InstallAsync();
    }

    /// <summary>
    /// Uninstall the plugin
    /// </summary>
    public override async Task UninstallAsync()
    {
        // Delete localization resources
        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Widgets.MaibaHeroSlider");

        await base.UninstallAsync();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
    /// </summary>
    public bool HideInWidgetList => false;

    #endregion
}

