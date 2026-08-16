using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Meio.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Meio.app;

public class App : Application
{
    private IHost _host = null!;

    public static ILogger Logger { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddHttpClient();
                services.AddPrettyConsole();
                services.AddMeioApi();
                services.AddCommonServices();
            })
            .Build();

        _host.StartAsync().GetAwaiter().GetResult();

        var loggerFactory = _host.Services.GetRequiredService<ILoggerFactory>();
        Logger = loggerFactory.CreateLogger("Meio.App");
        Logger.LogInformation("Meio Application started.");

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            desktop.MainWindow = mainWindow;

            desktop.Exit += async (_, _) => await _host.StopAsync();
        }

        base.OnFrameworkInitializationCompleted();
    }
}