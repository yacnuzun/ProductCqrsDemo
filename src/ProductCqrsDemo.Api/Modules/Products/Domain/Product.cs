public sealed class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    // EF Core materyalizasyon için gerekli, dışarıya kapalı.
    private Product() { }

    public static Product Create(string name, string? description, decimal price, int stock)
    {
        return new Product
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            Price = price,
            Stock = stock,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public void Update(string name, string? description, decimal price, int stock)
    {
        Name = name;
        Description = description;
        Price = price;
        Stock = stock;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}