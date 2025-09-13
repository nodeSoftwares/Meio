using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using PrettyLogging.Console;

namespace Meio.app;

public class App : Application
{
    private static ILoggerFactory? LoggerFactory { get; set; }

    public static ILogger<App>? Logger { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Create and configure PrettyLogger
        LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
        {
            builder.ClearProviders();
            builder.AddPrettyConsole(opt =>
            {
                opt.ShowLogLevel = true;
                opt.ShowEventId = false;
                opt.ShowManagedThreadId = false;
                opt.SingleLine = true;
                opt.IncludeScopes = false;
                opt.ShowTimestamp = true;
                opt.LogLevelCase = LogLevelCase.Upper;
                opt.CategoryMode = LoggerCategoryMode.Short;
                opt.ColorBehavior = LoggerColorBehavior.Enabled;
                opt.UseUtcTimestamp = false;
            });

#if DEBUG
            builder.SetMinimumLevel(LogLevel.Debug);
#else
            builder.SetMinimumLevel(LogLevel.Information);
#endif
        });

        Logger = LoggerFactory.CreateLogger<App>();
        Logger.LogInformation("Meio Application started.");

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}