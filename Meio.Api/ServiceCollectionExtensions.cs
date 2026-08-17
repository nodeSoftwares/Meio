using Meio.Api.Interfaces.Services;
using Meio.Api.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Meio.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMeioApi(this IServiceCollection services)
    {
        services.AddSingleton<IMeioLogger, MeioLogger>();
        services.AddSingleton<IAudioPlayerService, AudioPlayerService>();
        services.AddSingleton<IAudioMetadataService, AudioMetadataService>();
        return services;
    }
}