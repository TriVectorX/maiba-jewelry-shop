using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.MaibaHeroSlider.Models;

public partial record HeroSlideModel : BaseNopEntityModel
{
    public int DisplayOrder { get; set; }
    public string PictureUrl { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string Heading { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ButtonText { get; set; } = string.Empty;
    public string ButtonUrl { get; set; } = string.Empty;
}

