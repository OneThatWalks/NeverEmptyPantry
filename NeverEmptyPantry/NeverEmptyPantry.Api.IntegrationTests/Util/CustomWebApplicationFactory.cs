using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using NeverEmptyPantry.Common.Models.Entity;
using NeverEmptyPantry.Common.Models.Identity;
using NeverEmptyPantry.Repository.Entity;
using System;
using System.Linq;

namespace NeverEmptyPantry.Api.IntegrationTests.Util
{
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
    }

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

        public static void SeedTestUsers(UserManager<ApplicationUser> userManager)
        {
            IdentityResult result1;
            var user1 = userManager.FindByNameAsync("TestUser1").Result;
            var user2 = userManager.FindByNameAsync("TestUser2").Result;

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

            result1 = userManager.CreateAsync(user1, "Str0ngP@ssword").Result;

            if (result1.Succeeded)
            {
                userManager.AddToRoleAsync(user1, "Administrator").Wait();
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

            result2 = userManager.CreateAsync(user2, "Str0ngP@ssword").Result;

            if (!result2.Succeeded)
            {
                var resultErrors2 = result2.Errors.Select(err => err.Description);
                throw new Exception($"Seed Users failed. {string.Join(',', resultErrors2)}");
            }
        }
    }
}