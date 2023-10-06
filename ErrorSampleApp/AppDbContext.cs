using ErrorSampleApp.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ErrorSampleApp;

public class AppDbContext : DbContext
{
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // Configure your database connection here
        options.UseNpgsql("Server=localhost; port=5432; Username=postgres; Password=data-safety; Database=test_db;");
        options.LogTo(Console.WriteLine, LogLevel.Information);
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<ProductUnitLink> ProductUnitLinks { get; set; }
    public DbSet<ProductCp> ProductCps { get; set; }
    public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
}