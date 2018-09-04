using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NeverEmptyPantry.Common.Models;
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
        public DbSet<List> Lists { get; set; }
        public DbSet<ListProduct> ListProducts { get; set; }
        public DbSet<UserProductVote> UserProductVotes { get; set; }
        public DbSet<OfficeLocation> OfficeLocations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Product>().ToTable("Product");
            builder.Entity<List>().ToTable("List");
            builder.Entity<ListProduct>().ToTable("ListProduct");
            builder.Entity<UserProductVote>().ToTable("UserProductVote");
            builder.Entity<OfficeLocation>().ToTable("OfficeLocation");

        }
    }
}