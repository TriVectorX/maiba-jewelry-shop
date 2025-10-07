using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Plugin.Security.AdminAccess.Infrastructure;

public class PluginStartup : INopStartup
{
    public int Order => 11000;

    public void Configure(IApplicationBuilder application)
    {
    }

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Register the filter for admin area
        services.Configure<MvcOptions>(options =>
        {
            options.Filters.Add<AdminAccessFilter>(int.MinValue);
        });
    }
}

