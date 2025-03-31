using OnlineStore.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Reflection.Emit;


namespace OnlineStore.Models
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Ensures Identity models are configured properly
            modelBuilder.Entity<Product>()
           .Property(p => p.Price)
           .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany() // or .HasMany() if Category has a collection of products
            .HasForeignKey(p => p.CategoryId)
        .   OnDelete(DeleteBehavior.Restrict); // Restrict cascade delete

            // SubCategory Foreign Key: Prevent cascade delete
             modelBuilder.Entity<Product>()
                .HasOne(p => p.SubCategory)
                .WithMany() // or .HasMany() if SubCategory has a collection of products
                .HasForeignKey(p => p.SubCategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict cascade delete
        }


    }
}




