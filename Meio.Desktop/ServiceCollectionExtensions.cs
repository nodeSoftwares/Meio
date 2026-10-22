using System;
using Meio.desktop.Interfaces.Services;
using Meio.desktop.Services;
using Meio.desktop.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using PrettyLogging.Console;

namespace Meio.desktop;

public static class ServiceCollectionExtensions
{
    public static void AddPrettyConsole(this IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddPrettyConsole(opt =>
            {
                opt.ShowLogLevel = true;
                opt.ShowEventId = false;
                opt.ShowManagedThreadId = false;
                opt.SingleLine = true;
                opt.IncludeScopes = true;
                opt.ShowTimestamp = true;
                opt.LogLevelCase = LogLevelCase.Upper;
                opt.CategoryMode = LoggerCategoryMode.Short;
                opt.ColorBehavior = LoggerColorBehavior.Enabled;
                opt.UseUtcTimestamp = false;
            });

#if DEBUG
            builder.SetMinimumLevel(LogLevel.Trace);
#else
            builder.SetMinimumLevel(LogLevel.Information);
#endif
        });
    }

    public static void AddCommonServices(this IServiceCollection services)
    {
        // Services
        services.AddSingleton<IDiscordService, DiscordPresence>();
        services.AddSingleton<INavigationService, NavigationService>();

        // ViewModels
        services.AddSingleton<MainWindow>();
        services.AddTransient<HomeView>();
    }

    public static void AddHttpClient(this IServiceCollection services)
    {
        services.AddHttpClient("Meio",
            client =>
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Meio/1.0");
                client.Timeout = TimeSpan.FromSeconds(30);
            });
    }
}