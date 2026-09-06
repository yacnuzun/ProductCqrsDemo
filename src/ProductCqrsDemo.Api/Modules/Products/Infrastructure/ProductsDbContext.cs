using Microsoft.EntityFrameworkCore;

public sealed class ProductsDbContext : DbContext
{
    public ProductsDbContext(DbContextOptions<ProductsDbContext> options)
        : base(options) { }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Assembly'deki tüm IEntityTypeConfiguration'ları bulur.
        // Yeni entity eklediğinde buraya satır eklemene gerek kalmaz.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}