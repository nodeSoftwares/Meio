using System;
using Meio.Api;
using Meio.Api.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PrettyLogging.Console;

// TODO: Comment.

var host = Host.CreateDefaultBuilder(args).ConfigureServices((_, services) =>
{
    services.AddLogging(builder =>
    {
        builder.ClearProviders();
        builder.SetMinimumLevel(LogLevel.Debug);
        builder.AddPrettyConsole(opt =>
        {
            opt.ShowLogLevel = true;
            opt.SingleLine = true;
            opt.IncludeScopes = true;
            opt.LogLevelCase = LogLevelCase.Upper;
        });
    });

    services.AddHttpClient("Meio",
        client =>
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Meio/1.0");
            client.Timeout = TimeSpan.FromSeconds(30);
        });
    services.AddMeioApi();
}).Build();

host.Start();

var audioPlayerService = host.Services.GetRequiredService<IAudioPlayerService>();

Console.WriteLine("Playing example audio...");
audioPlayerService.Play("/example/audio/path");

host.WaitForShutdown();