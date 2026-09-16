using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/products")]
public sealed class ProductsController : ControllerBase
{
    private readonly ISender _sender;

    public ProductsController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetAll(CancellationToken ct)
        => Ok(await _sender.Send(new GetAllProductsQuery(), ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id, CancellationToken ct)
        => Ok(await _sender.Send(new GetProductByIdQuery(id), ct));

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
        [FromBody] CreateProductCommand command, CancellationToken ct)
    {
        var id = await _sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }
}