// ReSharper disable once CheckNamespace

using System;
using Microsoft.Extensions.Configuration;
using NeverEmptyPantry.Api.Interfaces;
using NeverEmptyPantry.Api.Util;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static INeverEmptyPantryBuilder AddNeverEmptyPantry(this IServiceCollection services, IConfiguration configuration)
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
            var builder = services.AddNeverEmptyPantryCore(configuration);

            // Additional generic services can be added here
            builder.Services.AddMvc();

            return builder;
        }

        public static INeverEmptyPantryBuilder AddNeverEmptyPantryCore(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var builder = new NeverEmptyPantryBuilder(services, configuration);

            builder.AddConnectionStrings();

            builder.AddJwtDetails();

            builder.AddDbContext();

            builder.AddIdentity();

            builder.AddJwtAuthentication();

            builder.AddRepository();

            builder.AddApplication();

            return builder;
        }
    }
}