using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.MaibaHeroSlider.Models;

public partial record ConfigurationModel : BaseNopModel
{
    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.MaibaHeroSlider.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.MaibaHeroSlider.Fields.PictureId")]
    public int PictureId { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.MaibaHeroSlider.Fields.ThumbnailPictureId")]
    public int ThumbnailPictureId { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.MaibaHeroSlider.Fields.Heading")]
    public string Heading { get; set; } = string.Empty;

    [NopResourceDisplayName("Plugins.Widgets.MaibaHeroSlider.Fields.Tag")]
    public string Tag { get; set; } = string.Empty;

    [NopResourceDisplayName("Plugins.Widgets.MaibaHeroSlider.Fields.Description")]
    public string Description { get; set; } = string.Empty;

    [NopResourceDisplayName("Plugins.Widgets.MaibaHeroSlider.Fields.ButtonText")]
    public string ButtonText { get; set; } = string.Empty;

    [NopResourceDisplayName("Plugins.Widgets.MaibaHeroSlider.Fields.ButtonUrl")]
    public string ButtonUrl { get; set; } = string.Empty;

    [NopResourceDisplayName("Plugins.Widgets.MaibaHeroSlider.Fields.IsActive")]
    public bool IsActive { get; set; }

    public IList<HeroSlideListModel> Slides { get; set; } = new List<HeroSlideListModel>();
}

public partial record HeroSlideListModel : BaseNopEntityModel
{
    public int DisplayOrder { get; set; }
    public string Heading { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

