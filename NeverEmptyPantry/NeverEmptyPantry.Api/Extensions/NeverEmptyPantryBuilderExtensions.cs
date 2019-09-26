using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NeverEmptyPantry.Api.Interfaces;
using NeverEmptyPantry.Api.Util;
using NeverEmptyPantry.Application.Services;
using NeverEmptyPantry.Application.Validators;
using NeverEmptyPantry.Common.Interfaces.Application;
using NeverEmptyPantry.Common.Interfaces.Repository;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Account;
using NeverEmptyPantry.Common.Models.Entity;
using NeverEmptyPantry.Common.Models.Identity;
using NeverEmptyPantry.Repository.Entity;
using NeverEmptyPantry.Repository.Services;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class NeverEmptyPantryBuilderExtensions
    {
        public static INeverEmptyPantryBuilder AddConnectionStrings(this INeverEmptyPantryBuilder builder)
        {
            var connectionStrings = new ConnectionStrings();
            builder.Configuration.GetSection("ConnectionStrings").Bind(connectionStrings);
            builder.Services.AddSingleton(connectionStrings);
            builder.ConnectionStrings = connectionStrings;

            return builder;
        }

        public static INeverEmptyPantryBuilder AddJwtDetails(this INeverEmptyPantryBuilder builder)
        {
            var jwtDetails = new JwtDetails();
            builder.Configuration.GetSection("JwtDetails").Bind(jwtDetails);
            builder.Services.AddSingleton(jwtDetails);
            builder.JwtDetails = jwtDetails;

            return builder;
        }

        public static INeverEmptyPantryBuilder AddDbContext(this INeverEmptyPantryBuilder builder, Action<DbContextOptionsBuilder> options)
        {
            builder.Services.AddDbContext<ApplicationDbContext>(options);

            return builder;
        }

        public static INeverEmptyPantryBuilder AddIdentity(this INeverEmptyPantryBuilder builder)
        {
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.ClaimsIdentity.UserIdClaimType = JwtRegisteredClaimNames.Sub;
                    options.ClaimsIdentity.UserNameClaimType = JwtRegisteredClaimNames.UniqueName;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager<ApplicationSignInManager>()
                .AddDefaultTokenProviders();

            return builder;
        }

        public static INeverEmptyPantryBuilder AddJwtAuthentication(this INeverEmptyPantryBuilder builder)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => Remove default claims

            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(config =>
                {
                    config.RequireHttpsMetadata = true;
                    config.SaveToken = true;
                    config.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = builder.JwtDetails.JwtIssuer,
                        ValidAudience = builder.JwtDetails.JwtAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.JwtDetails.JwtKey)),
                        ClockSkew = TimeSpan.Zero, // remove delay of expired token
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true
                    };
                });

            return builder;
        }

        public static INeverEmptyPantryBuilder AddRepository(this INeverEmptyPantryBuilder builder)
        {
            builder.Services.AddScoped<IAccountRepository, AccountRepository>();

            builder.Services.AddScoped<IRepository<OfficeLocation>, BaseEntityRepository<OfficeLocation>>();
            builder.Services.AddScoped<IRepository<Product>, BaseEntityRepository<Product>>();

            return builder;
        }

        public static INeverEmptyPantryBuilder AddApplication(this INeverEmptyPantryBuilder builder)
        {
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IValidator<ProfileModel>, ProfileValidator>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IValidatorFactory<Product>, BaseEntityValidatorFactory<Product>>();

            return builder;
        }

        public static INeverEmptyPantryBuilder AddValidators(this INeverEmptyPantryBuilder builder)
        {
            builder.Services.AddScoped<IValidator<Product>, CreateProductValidator>();
            builder.Services.AddScoped<IValidator<Product>, RemoveProductValidator>();
            builder.Services.AddScoped<IValidator<Product>, UpdateProductValidator>();

            return builder;
        }
    }
}