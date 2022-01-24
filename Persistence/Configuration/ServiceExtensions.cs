using Microsoft.Extensions.DependencyInjection;
using Persistence.Repositories;

namespace Persistence.Configuration
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services)
        {
            return services
                .AddRepository();
        }
        private static IServiceCollection AddRepository(this IServiceCollection services)
        {
            return services
                .AddSingleton<IFileRepository, FileRepository>();
        }
    }
}
