using Microsoft.EntityFrameworkCore;
using Sale.Share.Entities;

namespace Sale.Api.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options):base(options)
        {
                
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            DisableCascadingDelete(modelBuilder);
        }

        private void DisableCascadingDelete(ModelBuilder modelBuilder)
        {
            var relationship = modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys());
            foreach (var item in relationship)
            {
                item.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductsubCategory> productsubCategories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<SerialNumber> serialNumbers { get; set; }
        public DbSet<Brand> brands { get; set; }

        public DbSet<Subcategory> subcategories { get; set; }

    }
}
