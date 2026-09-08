using MediatR;

public sealed record CreateProductCommand(
    string Name,
    string? Description,
    decimal Price,
    int Stock) : IRequest<Guid>;