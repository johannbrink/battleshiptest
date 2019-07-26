using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Serilog;

namespace Battleship.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
                BuildWebHost2(args).Run();
        }

        public static IWebHost BuildWebHost2(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
                    .ReadFrom.Configuration(hostingContext.Configuration)
                    .Enrich.FromLogContext())
                .Build();
    }
}
