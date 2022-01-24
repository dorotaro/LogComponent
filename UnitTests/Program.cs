using Domain.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UnitTests
{
	public class Program
	{
		public ServiceProvider ServiceProvider { get; }
		public Program()
		{
			var services = new ServiceCollection();
			services.AddDomain();
			ServiceProvider = services.BuildServiceProvider();
		}
	}
}