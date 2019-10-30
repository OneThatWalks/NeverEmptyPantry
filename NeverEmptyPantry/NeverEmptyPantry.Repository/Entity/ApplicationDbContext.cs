using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Options;
using NeverEmptyPantry.Common.Enum;
using NeverEmptyPantry.Common.Models.Entity;
using NeverEmptyPantry.Common.Models.Identity;
using System;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using NeverEmptyPantry.Common.Permissions;

namespace NeverEmptyPantry.Repository.Entity
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<List> Lists { get; set; }
        public DbSet<ListProduct> ListProducts { get; set; }
        public DbSet<UserListProductVote> UserListProductVotes { get; set; }
        public DbSet<OfficeLocation> OfficeLocations { get; set; }
        public DbSet<AuditLog> AuditLog { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Product>()
                .ToTable("Product");

            builder.Entity<Category>()
                .ToTable("Categories");

            builder.Entity<List>()
                .ToTable("List")
                .Property(e => e.OrderState)
                .HasConversion(new EnumToStringConverter<OrderState>());

            builder.Entity<ListProduct>()
                .ToTable("ListProduct")
                .Property(e => e.ListProductState)
                .HasConversion(new EnumToStringConverter<ListProductState>());

            builder.Entity<UserListProductVote>()
                .ToTable("UserListProductVote")
                .Property(e => e.UserProductVoteState)
                .HasConversion(new EnumToStringConverter<UserProductVoteState>());

            builder.Entity<OfficeLocation>()
                .ToTable("OfficeLocation");

            builder.Entity<AuditLog>()
                .ToTable("AuditLog")
                .Property(e => e.AuditAction)
                .HasConversion(new EnumToStringConverter<AuditAction>());

            var indyOffice = new OfficeLocation()
            {
                Id = 1,
                Name = "Indy Office",
                Active = true,
                Address = "9025 River Road Suite 150",
                City = "Indianapolis",
                Country = "USA",
                CreatedDateTimeUtc = DateTime.UtcNow,
                State = "IN",
                Zip = "46240"
            };

            builder.Entity<OfficeLocation>()
                .HasData(indyOffice);
        }
    }

    public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>
    {
        public ApplicationUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor)
        {
        }
    }

    public static class ApplicationDbInitializer
    {
        public static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
        {
            if (await userManager.FindByNameAsync("System") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "System",
                    Email = "",
                    TwoFactorEnabled = false,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "qxUWa2SDUW[mptmw");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, DefaultRoles.Administrator);
                }
            }
        }

        public static async Task SeedRolesClaimsAsync(RoleManager<IdentityRole> roleManager)
        {
            var adminRole = await roleManager.FindByNameAsync(DefaultRoles.Administrator);
            var userRole = await roleManager.FindByNameAsync(DefaultRoles.User);

            if (adminRole == null)
            {
                var role = new IdentityRole(DefaultRoles.Administrator);
                await roleManager.CreateAsync(role);
            }

            if (userRole == null)
            {
                var role = new IdentityRole(DefaultRoles.User);
                await roleManager.CreateAsync(role);
            }
        }
    }
}