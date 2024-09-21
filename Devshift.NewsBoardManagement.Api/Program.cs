using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NLog.Web;
using Steeltoe.Discovery.Client;

namespace Devshift.NewsBoardManagement.Api {
    public static class Program {
        public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .UseKestrel(k =>
                        {
                            k.AddServerHeader = false;
                        });
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", false, false)
                        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", false, false)
                        // .AddJsonFile($"appsettings.Development.json", false, false)
                        .AddJsonFile("secrets.json", true, false)
                        .AddEnvironmentVariables();
                })
                // .AddDiscoveryClient()
                .UseNLog();
    }
}