using Microsoft.Extensions.DependencyInjection;

namespace RaspCameraLibrary.Services;

/// <summary>
/// DI registration helpers.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register <see cref="ICameraService"/> as singleton.
    /// </summary>
    public static IServiceCollection AddRaspCamera(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddSingleton<ICameraService, CameraService>();
        return services;
    }
}
