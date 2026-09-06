public sealed record ProductDto(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    int Stock,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);