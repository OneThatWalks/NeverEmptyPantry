using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace NeverEmptyPantry.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // create the web host builder
            CreateWebHostBuilder(args)
                // build the web host
                .Build()
                // and run the web host, i.e. your web application
                .Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            // create a default web host builder, with the default settings and configuration
            WebHost.CreateDefaultBuilder(args)
                // configure it to use your `Startup` class
                .UseStartup<Startup>();
    }
}
