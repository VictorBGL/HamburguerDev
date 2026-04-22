using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace HamburguerDev.Shared.CommonConfigurations;

public static class SharedConfigurations
{
    public static WebApplicationBuilder AddSharedConfiguration(this WebApplicationBuilder builder)
    {
        var sharedSettingsPath = builder.Configuration["SharedSettingsPath"];

        if (!string.IsNullOrWhiteSpace(sharedSettingsPath))
        {
            var sharedSettingsFullPath = Path.Combine(builder.Environment.ContentRootPath, sharedSettingsPath);
            builder.Configuration.AddJsonFile(sharedSettingsFullPath, optional: false, reloadOnChange: true);
        }

        return builder;
    }
}
