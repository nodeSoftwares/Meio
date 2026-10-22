using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Meio.Api;
using Meio.desktop.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Meio.desktop;

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
            }).UseConsoleLifetime()
            .Build();

        _host.StartAsync().GetAwaiter().GetResult();

        var loggerFactory = _host.Services.GetRequiredService<ILoggerFactory>();
        Logger = loggerFactory.CreateLogger("Meio.App");
        Logger.LogInformation("Meio Application started.");

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            desktop.MainWindow = mainWindow;

            desktop.Exit += (_, _) =>
            {
                _host.StopAsync().GetAwaiter().GetResult();
                _host.Dispose();
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}