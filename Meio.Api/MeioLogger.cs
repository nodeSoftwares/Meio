using System;
using Microsoft.Extensions.Logging;

namespace Meio.Api;

public interface IMeioLogger
{
    public void LogDebug(string message, params object[] args);

    public void LogInformation(string message, params object[] args);

    public void LogWarning(string message, params object[] args);

    public void LogError(string message, params object[] args);

    public void LogError(Exception ex, string message, params object[] args);
}

internal sealed class MeioLogger(ILoggerFactory loggerFactory) : IMeioLogger
{
    private readonly ILogger _logger = loggerFactory.CreateLogger("Meio.Api");

    public void LogDebug(string message, params object[] args)
    {
        _logger.LogDebug(message, args);
    }

    public void LogInformation(string message, params object[] args)
    {
        _logger.LogInformation(message, args);
    }

    public void LogWarning(string message, params object[] args)
    {
        _logger.LogWarning(message, args);
    }

    public void LogError(string message, params object[] args)
    {
        _logger.LogError(message, args);
    }

    public void LogError(Exception ex, string message, params object[] args)
    {
        _logger.LogError(ex, message, args);
    }
}