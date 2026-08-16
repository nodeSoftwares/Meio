using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace Meio.Api.Interfaces;

public interface IMeioApi
{
    HttpClient HttpClient { get; }

    ILogger Logger { get; }
}