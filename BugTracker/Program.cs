using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace BugTracker
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
		    Host.CreateDefaultBuilder(args)
			  .ConfigureWebHostDefaults(webBuilder =>
			  {
				  webBuilder.UseStartup<Startup>();
			  })
			.ConfigureLogging((hostingContext, logging) =>
			{
				logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
				logging.AddConsole();
				logging.AddDebug();
				logging.AddEventSourceLogger();
				logging.AddNLog();
			});
	}
}
