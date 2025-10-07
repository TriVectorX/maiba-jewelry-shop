using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.MaibaHeroSlider.Models;

public partial record PublicInfoModel : BaseNopModel
{
    public IList<HeroSlideModel> Slides { get; set; } = new List<HeroSlideModel>();
}

