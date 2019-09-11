using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Options;
using NeverEmptyPantry.Common.Enum;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Entity;
using NeverEmptyPantry.Common.Models.Identity;
using NeverEmptyPantry.Common.Models.List;
using NeverEmptyPantry.Common.Models.Product;

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
        public DbSet<ListProductMap> ListProducts { get; set; }
        public DbSet<UserProductVote> UserProductVotes { get; set; }
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

            builder.Entity<ListProductMap>()
                .ToTable("ListProduct")
                .Property(e => e.ListProductState)
                .HasConversion(new EnumToStringConverter<ListProductState>());

            builder.Entity<UserProductVote>()
                .ToTable("UserProductVote")
                .Property(e => e.UserProductVoteState)
                .HasConversion(new EnumToStringConverter<UserProductVoteState>());

            builder.Entity<OfficeLocation>()
                .ToTable("OfficeLocation");

            builder.Entity<AuditLog>()
                .ToTable("AuditLog")
                .Property(e => e.AuditAction)
                .HasConversion(new EnumToStringConverter<AuditAction>());
        }
    }

    public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>
    {
        public ApplicationUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor)
        {
        }
    }
}