using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NeverEmptyPantry.Common.Models;

namespace NeverEmptyPantry.Api.Interfaces
{
    public interface INeverEmptyPantryBuilder
    {
        IServiceCollection Services { get; }

        IConfiguration Configuration { get; }

        ConnectionStrings ConnectionStrings { get; set; }

        JwtDetails JwtDetails { get; set; }
    }
}