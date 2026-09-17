using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class DeleteProductCommandHandler
    : IRequestHandler<DeleteProductCommand>
{
    private readonly ProductsDbContext _db;

    public DeleteProductCommandHandler(ProductsDbContext db) => _db = db;

    public async Task Handle(DeleteProductCommand request, CancellationToken ct)
    {
        var product = await _db.Products
            .FirstOrDefaultAsync(p => p.Id == request.Id, ct)
            ?? throw new NotFoundException("Product", request.Id);

        _db.Products.Remove(product);
        await _db.SaveChangesAsync(ct);
    }
}