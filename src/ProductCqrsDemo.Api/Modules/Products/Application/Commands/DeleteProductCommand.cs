using MediatR;

public sealed record DeleteProductCommand(Guid Id) : IRequest;