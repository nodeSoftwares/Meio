using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using Meio.app.Views;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using PrettyLogging.Console;

namespace Meio.app;

public class App : Application
{
    private Styles _darkStyle;
    private Styles _lightStyle;
    private Styles _style;

    private static ILoggerFactory? LoggerFactory { get; set; }

    public static ILogger<App>? Logger { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        _lightStyle =
        [
            new StyleInclude(new Uri("resm:Styles?assembly=Meio.App"))
            {
                Source = new Uri("avares://Meio.App/Resources/Styles/LightThemeStyles.axaml")
            }
        ];

        _darkStyle =
        [
            new StyleInclude(new Uri("resm:Styles?assembly=Meio.App"))
            {
                Source = new Uri("avares://Meio.App/Resources/Styles/DarkThemeStyles.axaml")
            }
        ];

        _style =
        [
            new StyleInclude(new Uri("resm:Styles?assembly=Meio.App"))
            {
                Source = new Uri("avares://Meio.App/Resources/Styles/Styles.axaml")
            }
        ];
    }

    public void SwitchToLight()
    {
        Styles.Clear();
        Styles.Add(_style);
        Styles.Add(_lightStyle);
        Logger!.LogDebug("Changed application theme to light theme");
    }

    public void SwitchToDark()
    {
        Styles.Clear();
        Styles.Add(_style);
        Styles.Add(_darkStyle);
        Logger!.LogDebug("Changed application theme to dark theme");
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

        Logger = LoggerFactory.CreateLogger<App>();
        Logger.LogInformation("Meio Application started.");

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}