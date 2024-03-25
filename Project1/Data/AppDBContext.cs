using Microsoft.EntityFrameworkCore;
using Project1.Models.DBModels;

namespace Project1.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> option):base(option) { }

        public DbSet<ProductModel> Product { get; set; }
        public DbSet<CategoryModel> Category { get; set; }
        public DbSet<UserModel> User { get; set; }
        public DbSet<Basket> Basket { get; set; }
        public DbSet<Purchases> Purchases { get; set; }
        public DbSet<Views> Views { get; set; }
        public DbSet<Comments> Comments { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductModel>()
                .HasOne(p => p.Category)
                .WithMany(c => c.products)  // указываем навигационное свойство в обратном направлении
                .HasForeignKey(p => p.IdCategory);

            modelBuilder.Entity<Basket>()
                .HasKey(b => new { b.IdUser, b.IdProduct });

            modelBuilder.Entity<Basket>()
                .HasOne(b => b.User)
                .WithMany(u => u.Basket)
                .HasForeignKey(b => b.IdUser);

            modelBuilder.Entity<Basket>()
                .HasOne(b => b.Product)
                .WithMany(p => p.Basket)
                .HasForeignKey(b => b.IdProduct);

            modelBuilder.Entity<Purchases>()
                .HasKey(b => new { b.IdUser, b.IdProduct });

            modelBuilder.Entity<Purchases>()
                .HasOne(b => b.User)
                .WithMany(u => u.Purchases)
                .HasForeignKey(b => b.IdUser);

            modelBuilder.Entity<Purchases>()
                .HasOne(b => b.Product)
                .WithMany(p => p.Purchases)
                .HasForeignKey(b => b.IdProduct);

            modelBuilder.Entity<Views>()
               .HasKey(b => new { b.IdUser, b.IdProduct });

            modelBuilder.Entity<Views>()
                .HasOne(b => b.User)
                .WithMany(u => u.Views)
                .HasForeignKey(b => b.IdUser);

            modelBuilder.Entity<Views>()
                .HasOne(b => b.Product)
                .WithMany(p => p.Views)
                .HasForeignKey(b => b.IdProduct);

            modelBuilder.Entity<Comments>()
                .HasKey(c => c.IdComments);

            modelBuilder.Entity<Comments>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.IdUser);

            modelBuilder.Entity<Comments>()
                .HasOne(c => c.Product)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.IdProduct);

            base.OnModelCreating(modelBuilder);
        }
    }
}
