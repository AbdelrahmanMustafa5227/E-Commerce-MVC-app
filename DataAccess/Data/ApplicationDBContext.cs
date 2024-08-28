using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DataAccess.Data
{
    public class ApplicationDBContext : IdentityDbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
                
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // only written when using IdentityDbContext not DbContext
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1 , name = "Action" ,displayOrder = 1},
                new Category { Id = 2, name = "SciFi", displayOrder = 1 },
                new Category { Id = 3, name = "History", displayOrder = 1 }
                );

            modelBuilder.Entity<Product>().HasData(
                new Product {
                    Id = 1,
                    Title = "One Man's Sky",
                    Author = "Billy Spark",
                    Description = "akdjaks asldijaslid asldijalidjgubr ggkruggd slfoksel",
                    ISBN = "SWD999921",
                    ListPrice = 99,
                    Price = 90,
                    Price50 = 85,
                    Price100 = 80,
                    Category_Id = 1,
                    ImageURL=""
                },
                new Product
                {
                    Id = 2,
                    Title = "Rocky",
                    Author = "Jason Brody",
                    Description = "akdjaks asldijaslid asldijalidjgubr ggkruggd slfoksel",
                    ISBN = "SWD999451",
                    ListPrice = 80,
                    Price = 75,
                    Price50 = 70,
                    Price100 = 65,
                    Category_Id = 3,
                    ImageURL = ""
                },
                new Product
                {
                    Id = 3,
                    Title = "Invisible",
                    Author = "Tommy Frag",
                    Description = "akdjaks asldijaslid asldijalidjgubr ggkruggd slfoksel",
                    ISBN = "SWD993321",
                    ListPrice = 40,
                    Price = 45,
                    Price50 = 40,
                    Price100 = 35,
                    Category_Id = 2,
                    ImageURL = ""
                },
                new Product
                {
                    Id = 4,
                    Title = "Sambo",
                    Author = "Abella Danger",
                    Description = "akdjaks asldijaslid asldijalidjgubr ggkruggd slfoksel",
                    ISBN = "SWD991911",
                    ListPrice = 120,
                    Price = 100,
                    Price50 = 85,
                    Price100 = 80,
                    Category_Id = 3,
                    ImageURL = ""
                },
                new Product
                {
                    Id = 5,
                    Title = "Lust",
                    Author = "Franky Modest",
                    Description = "akdjaks asldijaslid asldijalidjgubr ggkruggd slfoksel",
                    ISBN = "SWD941121",
                    ListPrice = 99,
                    Price = 90,
                    Price50 = 85,
                    Price100 = 80,
                    Category_Id = 4,
                    ImageURL = ""
                }
            );
        }
    }
}
