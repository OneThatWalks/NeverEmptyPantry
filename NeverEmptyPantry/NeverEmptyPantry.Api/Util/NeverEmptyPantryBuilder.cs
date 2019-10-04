using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NeverEmptyPantry.Api.Interfaces;
using NeverEmptyPantry.Common.Models;
using System;
using System.Diagnostics.CodeAnalysis;

namespace NeverEmptyPantry.Api.Util
{
    [ExcludeFromCodeCoverage]
    public class NeverEmptyPantryBuilder : INeverEmptyPantryBuilder
    {
        public IServiceCollection Services { get; }
        public IConfiguration Configuration { get; }
        public ConnectionStrings ConnectionStrings { get; set; }
        public JwtDetails JwtDetails { get; set; }

        public NeverEmptyPantryBuilder(IServiceCollection services, IConfiguration configuration)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            Configuration = configuration;
        }
    }
}