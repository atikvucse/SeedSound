using Microsoft.Extensions.DependencyInjection;
using SeedSound.Core.Services;
using SeedSound.Infrastructure.Services;

namespace SeedSound.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IMusicGeneratorService, MusicGeneratorService>();
        services.AddSingleton<ISongGeneratorService, SongGeneratorService>();
        services.AddSingleton<IExportService, ExportService>();

        return services;
    }
}
