using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WhatsMyRecipe.Models;

namespace WhatsMyRecipe.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>()
                .Property(c => c.UserId)
                .IsRequired(false);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Salad", IsCustom = false, UserId = null, Image = "/images/categories/salad.jpg" },
                new Category { Id = 2, Name = "Meat", IsCustom = false, UserId = null, Image = "/images/categories/meat.jpg" },
                new Category { Id = 3, Name = "Dessert", IsCustom = false, UserId = null, Image = "/images/categories/dessert.jpg" },
                new Category { Id = 4, Name = "Drink", IsCustom = false, UserId = null, Image = "/images/categories/drink.jpg" },
                new Category { Id = 5, Name = "Appetizer", IsCustom = false, UserId = null, Image = "/images/categories/appetizers.jpg" }, 
                new Category { Id = 6, Name = "Vegan", IsCustom = false, UserId = null, Image = "/images/categories/vegan.jpg" }
            );
        }
    }
}