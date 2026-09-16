using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class GetAllProductsQueryHandler
    : IRequestHandler<GetAllProductsQuery, IReadOnlyList<ProductDto>>
{
    private readonly ProductsDbContext _db;

    public GetAllProductsQueryHandler(ProductsDbContext db) => _db = db;

    public async Task<IReadOnlyList<ProductDto>> Handle(
        GetAllProductsQuery request, CancellationToken ct)
    {
        return await _db.Products
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAtUtc)
            .Select(p => new ProductDto(
                p.Id, p.Name, p.Description, p.Price, p.Stock, p.CreatedAtUtc, p.UpdatedAtUtc))
            .ToListAsync(ct);
    }
}