using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;

namespace Beis.HelpToGrow.Web
{
    [ExcludeFromCodeCoverage]
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
                }).ConfigureAppConfiguration(configBuilder =>
                {
                    var connectionString = configBuilder.Build().GetConnectionString("AppConfig");
                    if (connectionString != null)
                    {
                        configBuilder.AddAzureAppConfiguration(connectionString);
                    }
                });
    }
}