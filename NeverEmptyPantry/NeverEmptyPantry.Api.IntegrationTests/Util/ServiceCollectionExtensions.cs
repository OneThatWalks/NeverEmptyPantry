using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NeverEmptyPantry.Api.Interfaces;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static INeverEmptyPantryBuilder AddNeverEmptyPantryIntegrationTesting(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration == null)
            {
                throw new ArgumentException(nameof(configuration));
            }

            // Creates the builder with all required NEP services
            var builder = services.AddNeverEmptyPantryCore(configuration, options =>
            {
                options.UseInMemoryDatabase("InMemoryDatabase");
            });

            // Additional generic services can be added here
            builder.Services.AddMvc()
                .AddApplicationPart(Assembly.Load(new AssemblyName("NeverEmptyPantry.Api")))
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            return builder;
        }
    }
}