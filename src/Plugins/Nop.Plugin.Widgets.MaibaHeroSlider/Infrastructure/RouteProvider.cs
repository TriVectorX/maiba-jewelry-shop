using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Widgets.MaibaHeroSlider.Infrastructure;

/// <summary>
/// Represents plugin route provider
/// </summary>
public partial class RouteProvider : IRouteProvider
{
    /// <summary>
    /// Register routes
    /// </summary>
    /// <param name="endpointRouteBuilder">Route builder</param>
    public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapControllerRoute(MaibaHeroSliderDefaults.ConfigurationRouteName,
            "Admin/WidgetMaibaHeroSlider/Configure",
            new { controller = "MaibaHeroSlider", action = "Configure", area = "Admin" });
    }

    /// <summary>
    /// Gets a priority of route provider
    /// </summary>
    public int Priority => 0;
}

