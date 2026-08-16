using System;
using System.Net.Http;
using Meio.Api.Interfaces;
using Microsoft.Extensions.Logging;

namespace Meio.Api;

internal class MeioApi(HttpClient httpClient, ILogger<MeioApi> logger) : IMeioApi, IDisposable
{
    public void Dispose()
    {
        HttpClient.Dispose();
    }

    public HttpClient HttpClient { get; } = httpClient;

    public ILogger Logger { get; } = logger;

    public void Init()
    {
        Logger.LogInformation("Meio API started.");
    }
}