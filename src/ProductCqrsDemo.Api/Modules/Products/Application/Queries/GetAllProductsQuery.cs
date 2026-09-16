using MediatR;

public sealed record GetAllProductsQuery : IRequest<IReadOnlyList<ProductDto>>;