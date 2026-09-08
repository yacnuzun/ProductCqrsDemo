using MediatR;

public sealed class CreateProductCommandHandler
    : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly ProductsDbContext _db;

    public CreateProductCommandHandler(ProductsDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var product = Product.Create(
            request.Name,
            request.Description,
            request.Price,
            request.Stock);

        _db.Products.Add(product);
        await _db.SaveChangesAsync(ct);

        return product.Id;
    }
}