using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class GetProductByIdQueryHandler
    : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly ProductsDbContext _db;

    public GetProductByIdQueryHandler(ProductsDbContext db) => _db = db;

    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        var dto = await _db.Products
            .AsNoTracking()
            .Where(p => p.Id == request.Id)
            .Select(p => new ProductDto(
                p.Id, p.Name, p.Description, p.Price, p.Stock, p.CreatedAtUtc, p.UpdatedAtUtc))
            .FirstOrDefaultAsync(ct);

        return dto ?? throw new NotFoundException("Product", request.Id);
    }
}