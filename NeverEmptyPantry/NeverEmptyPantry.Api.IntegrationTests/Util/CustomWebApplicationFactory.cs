using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using NeverEmptyPantry.Common.Models.Entity;
using NeverEmptyPantry.Common.Models.Identity;
using NeverEmptyPantry.Repository.Entity;

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
        public static void PopulateTestData(ApplicationDbContext dbContext)
        {
            // Categories
            Category testCategory = new Category()
            {
                Name = "Test Category",
                Id = 1
            };

            dbContext.Categories.Add(testCategory);

            // Products
            Product testProduct = new Product() {
                Name = "Test product",
                Id = 1,
                Active = true,
                Category = testCategory,
            };

            dbContext.Products.Add(testProduct);

            // Save
            dbContext.SaveChanges();
        }

        public static void SeedTestUsers(UserManager<ApplicationUser> userManager)
        {
            ApplicationUser user1 = new ApplicationUser
            {
                UserName = "TestUser1",
                Email = "TestUser1@email.com",
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "18005555555",
                Title = "Tester"
            };

            ApplicationUser user2 = new ApplicationUser
            {
                UserName = "TestUser2",
                Email = "TestUser2@email.com",
                FirstName = "Jane",
                LastName = "Doe",
                PhoneNumber = "18005555555",
                Title = "Tester"
            };

            IdentityResult result1 = userManager.CreateAsync(user1, "Str0ngP@ssword").Result;
            IdentityResult result2 = userManager.CreateAsync(user2, "Str0ngP@ssword").Result;

            if (result1.Succeeded)
            {
                userManager.AddToRoleAsync(user1, "Administrator").Wait();
            }
            else if (!result1.Succeeded || !result2.Succeeded)
            {
                throw new Exception("Seed Users failed");
            }
        }
    }
}