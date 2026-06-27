using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using CartFlow.Data.Entities;

namespace CartFlow.Data.Data;

public class AppDbContext : DbContext {
    public DbSet<Order> Orders { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<Review> Reviews { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        base.OnConfiguring(optionsBuilder);

        var assemblyDir = System.IO.Path.GetDirectoryName(typeof(AppDbContext).Assembly.Location);
        var config = new ConfigurationBuilder()
            .SetBasePath(assemblyDir)
            .AddJsonFile(System.IO.Path.Combine("Data", "appsettings.json"), optional: false, reloadOnChange: true)
            .Build();

        var connectionString = config.GetSection("constr").Value;

        optionsBuilder.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
