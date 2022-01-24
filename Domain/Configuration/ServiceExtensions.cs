using Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Configuration;

namespace Domain.Configuration
{
    public static class ServiceExtensions
    {
		public static IServiceCollection AddDomain(this IServiceCollection service)
		{
			return service
				.AddServices()
				.AddPersistenceServices();
		}
		private static IServiceCollection AddServices(this IServiceCollection service)
		{
			return service
				.AddSingleton<ILogService, LogService>();
		}

		private static IServiceCollection AddPersistenceServices(this IServiceCollection service)
		{
			return service.AddPersistence();
		}
	}
}
