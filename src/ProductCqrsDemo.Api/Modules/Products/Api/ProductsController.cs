using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/products")]
public sealed class ProductsController : ControllerBase
{
    private readonly ISender _sender;

    // ISender = IMediator'ın sadece Send() kısmı.
    // Controller Publish() kullanmadığı için dar arayüzü alıyoruz.
    public ProductsController(ISender sender) => _sender = sender;

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
        [FromBody] CreateProductCommand command, CancellationToken ct)
    {
        var id = await _sender.Send(command, ct);
        return Created($"/api/products/{id}", id);
    }
}