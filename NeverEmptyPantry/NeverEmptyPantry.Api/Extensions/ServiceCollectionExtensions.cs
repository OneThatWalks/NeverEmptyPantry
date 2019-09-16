// ReSharper disable once CheckNamespace

using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            builder.Services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

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

            builder.AddDbContext(
                options =>
                    options.UseSqlServer(builder.ConnectionStrings.DefaultDbConnection));

            builder.AddIdentity();

            builder.AddJwtAuthentication();

            builder.AddRepository();

            builder.AddApplication();

            return builder;
        }

        public static INeverEmptyPantryBuilder AddNeverEmptyPantryCore(this IServiceCollection services, IConfiguration configuration, Action<DbContextOptionsBuilder> options)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var builder = new NeverEmptyPantryBuilder(services, configuration);

            builder.AddConnectionStrings();

            builder.AddJwtDetails();

            builder.AddDbContext(options);

            builder.AddIdentity();

            builder.AddJwtAuthentication();

            builder.AddRepository();

            builder.AddApplication();

            return builder;
        }
    }
}