using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using PrettyLogging.Console;

namespace Meio.Api;

public class Api
{
    private static ILoggerFactory? LoggerFactory { get; set; }

    public static ILogger<Api>? Logger { get; private set; }

    [ModuleInitializer]
    public static void Init()
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

        Logger = LoggerFactory.CreateLogger<Api>();
        Logger.LogInformation("Meio API started.");
    }

    public static void Main(string[] args)
    {
        Init();
    }
}