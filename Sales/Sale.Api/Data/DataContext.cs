using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sale.Share.Entities;

namespace Sale.Api.Data
{
    public class DataContext: IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options):base(options)
        {
                
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductsubCategory> productsubCategories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<SerialNumber> serialNumbers { get; set; }
        public DbSet<Brand> brands { get; set; }
        public DbSet<Subcategory> subcategories { get; set; }
        public DbSet<Country> countries { get; set; }
        public DbSet<State> states { get; set; }
        public DbSet<City> cities { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Country>().HasIndex(country => country.Name).IsUnique();           
            modelBuilder.Entity<State>().HasIndex(state => state.Name).IsUnique();
            modelBuilder.Entity<City>().HasIndex(city => city.Name).IsUnique();

            modelBuilder.Entity<Subcategory>()
                     .HasOne(sc => sc.Category)
                     .WithMany(c => c.subcategories)
                     .HasForeignKey(sc => sc.CategoryId)
                     .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Brand>()
                        .HasOne(p => p.Subcategory)
                        .WithMany(sc => sc.Brands)
                        .HasForeignKey(p => p.SubcategoryId)
                        .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Product>()
                         .HasOne(p => p.brand)
                         .WithMany(b => b.Products) 
                         .HasForeignKey(p => p.BrandId)
                         .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Product>()
                        .HasMany(p => p.ProductImages)
                        .WithOne(pi => pi.Product)
                        .HasForeignKey(pi => pi.ProductId)
                        .OnDelete(DeleteBehavior.Cascade);
                        modelBuilder.Entity<Product>()
                        .HasMany(p => p.productsubCategories)
                        .WithOne(sc => sc.Product) 
                        .HasForeignKey(sc => sc.ProductId)
                        .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Product>()
                        .HasMany(p => p.serialNumbers)
                        .WithOne(sn => sn.Product) 
                        .HasForeignKey(sn => sn.ProductId)
                        .OnDelete(DeleteBehavior.Cascade);
        }

        private void DisableCascadingDelete(ModelBuilder modelBuilder)
        {
            var relationship = modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys());
            foreach (var item in relationship)
            {
                item.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }      

    }
}
