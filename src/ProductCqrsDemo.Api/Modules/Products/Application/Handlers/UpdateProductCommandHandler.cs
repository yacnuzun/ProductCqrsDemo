using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class UpdateProductCommandHandler
    : IRequestHandler<UpdateProductCommand>
{
    private readonly ProductsDbContext _db;

    public UpdateProductCommandHandler(ProductsDbContext db) => _db = db;

    public async Task Handle(UpdateProductCommand request, CancellationToken ct)
    {
        // AsNoTracking YOK — entity'yi değiştireceğiz, EF'in takip etmesi gerekiyor.
        var product = await _db.Products
            .FirstOrDefaultAsync(p => p.Id == request.Id, ct)
            ?? throw new NotFoundException("Product", request.Id);

        product.Update(
            request.Name,
            request.Description,
            request.Price,
            request.Stock);

        await _db.SaveChangesAsync(ct);
    }
}