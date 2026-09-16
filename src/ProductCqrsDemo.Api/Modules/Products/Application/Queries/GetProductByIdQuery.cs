using MediatR;

public sealed record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;