using Domain.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LogComponent.Configuration
{
    public class ConfigurationExtensions
    {
        public IServiceProvider ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddDomain();

            services.AddSingleton<LoggerApp>();

            return services.BuildServiceProvider();
        }
    }
}
