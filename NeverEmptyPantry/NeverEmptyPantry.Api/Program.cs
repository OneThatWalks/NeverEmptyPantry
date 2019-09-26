using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace NeverEmptyPantry.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // create the web host builder
            CreateHostBuilder(args)
                // build the web host
                .Build()
                // and run the web host, i.e. your web application
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(serverOptions =>
                        {
                            // Set properties and call methods on options
                        })
                        .UseStartup<Startup>();
                });
    }
}
