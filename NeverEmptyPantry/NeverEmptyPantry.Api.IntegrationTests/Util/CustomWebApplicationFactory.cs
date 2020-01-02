using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NeverEmptyPantry.Common.Models.Entity;
using NeverEmptyPantry.Common.Models.Identity;
using NeverEmptyPantry.Repository.Entity;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using NeverEmptyPantry.Authorization.Permissions;
using System.Security.Claims;

namespace NeverEmptyPantry.Api.IntegrationTests.Util
{
    [ExcludeFromCodeCoverage]
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder(null)
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddDebug();
                })
                .UseStartup<TStartup>();
        }

        protected override TestServer CreateServer(IWebHostBuilder builder)
        {
            var server = base.CreateServer(builder);

            using var scope = server.Host.Services.CreateScope();

            var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();

            ApplicationDbInitializer.SeedRolesClaimsAsync(roleManager).GetAwaiter().GetResult();

            ApplicationDbInitializer.SeedUsersAsync(userManager).GetAwaiter().GetResult();

            SeedData.SeedTestUsersAsync(userManager).GetAwaiter().GetResult();

            return server;
        }
    }

    [ExcludeFromCodeCoverage]
    public static class SeedData
    {
        // TODO: Separate
        public static void PopulateTestData(ApplicationDbContext dbContext)
        {
            // TODO: Make these objects easier to access to assertions

            // Categories
            Category testCategory = dbContext.Categories.FirstOrDefault(c => c.Name.Equals("Test Category", StringComparison.CurrentCulture));
            if (testCategory == null)
            {
                testCategory = new Category()
                {
                    Name = "Test Category",
                    Id = 1
                };

                dbContext.Categories.Add(testCategory);
            }

            // Products
            Product testProduct =
                dbContext.Products.FirstOrDefault(p => p.Name.Equals("Test Product", StringComparison.CurrentCulture));
            if (testProduct == null)
            {
                testProduct = new Product()
                {
                    Name = "Test Product",
                    Id = 1,
                    Active = true,
                    Category = testCategory,
                };

                dbContext.Products.Add(testProduct);
            }

            // Save
            dbContext.SaveChanges();
        }

        public static async Task SeedTestUsersAsync(UserManager<ApplicationUser> userManager)
        {
            IdentityResult result1;
            var user1 = await userManager.FindByNameAsync("TestUser1");
            var user2 = await userManager.FindByNameAsync("TestUser2");

            if (user1 != null)
            {
                return;
            }

            user1 = new ApplicationUser
            {
                UserName = "TestUser1",
                Email = "TestUser1@email.com",
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "18005555555",
                Title = "Tester"
            };

            result1 = await userManager.CreateAsync(user1, "Str0ngP@ssword");

            if (result1.Succeeded)
            {
                await userManager.AddToRoleAsync(user1, DefaultRoles.Administrator);
            }
            else if (!result1.Succeeded)
            {
                var resultErrors1 = result1.Errors.Select(err => err.Description);
                throw new Exception($"Seed Users failed. {string.Join(',', resultErrors1)}");
            }
            IdentityResult result2 = null;

            if (user2 != null)
            {
                return;
            }

            user2 = new ApplicationUser
            {
                UserName = "TestUser2",
                Email = "TestUser2@email.com",
                FirstName = "Jane",
                LastName = "Doe",
                PhoneNumber = "18005555555",
                Title = "Tester"
            };

            result2 = await userManager.CreateAsync(user2, "Str0ngP@ssword");

            if (!result2.Succeeded)
            {
                var resultErrors2 = result2.Errors.Select(err => err.Description);
                throw new Exception($"Seed Users failed. {string.Join(',', resultErrors2)}");
            }
        }

        public static async Task SeedTestRolesAndPermissions(RoleManager<IdentityRole> roleManager)
        {
            var testRole1 = await roleManager.FindByNameAsync("TestRole1");
            var testRole2 = await roleManager.FindByNameAsync("TestRole2");

            var testRole1ExpectedClaims = new List<string>
            {
                Permissions.Users.Delete
            };

            var testRole2ExpectedClaims = new List<string>
            {
                Permissions.Users.Create
            };

            if (testRole1 == null)
            {
                var identityResult = await roleManager.CreateAsync(new IdentityRole
                {
                    Name = "TestRole1",
                    Id = "TestRole1"
                });

                if (identityResult.Succeeded == false)
                {
                    throw new Exception(identityResult.Errors.First().Description);
                }
            }

            if (testRole2 == null)
            {
                var identityResult = await roleManager.CreateAsync(new IdentityRole
                {
                    Name = "TestRole2",
                    Id = "TestRole2"
                });

                if (identityResult.Succeeded == false)
                {
                    throw new Exception(identityResult.Errors.First().Description);
                }
            }

            var testRole1Claims = await roleManager.GetClaimsAsync(testRole1);
            var testRole2Claims = await roleManager.GetClaimsAsync(testRole1);

            if (testRole1ExpectedClaims.Any(expected => !testRole1Claims.Any(actual => actual.Value.Equals(expected))))
            {
                var identityResults = testRole1ExpectedClaims.Select(c => roleManager.AddClaimAsync(testRole1, new Claim(CustomClaimTypes.Permission, c)));

                await Task.WhenAll(identityResults);

                if (identityResults.Any(i => !i.Result.Succeeded))
                {
                    throw new Exception(identityResults.First(i => i.Result.Errors.Any()).Result.Errors.First().Description);
                }
            }

            if (testRole2ExpectedClaims.Any(expected => !testRole2Claims.Any(actual => actual.Value.Equals(expected))))
            {
                var identityResults = testRole2ExpectedClaims.Select(c => roleManager.AddClaimAsync(testRole2, new Claim(CustomClaimTypes.Permission, c)));

                await Task.WhenAll(identityResults);

                if (identityResults.Any(i => !i.Result.Succeeded))
                {
                    throw new Exception(identityResults.First(i => i.Result.Errors.Any()).Result.Errors.First().Description);
                }
            }
        }
    }
}