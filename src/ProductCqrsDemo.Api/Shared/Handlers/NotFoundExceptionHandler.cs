using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

public sealed class NotFoundExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken ct)
    {
        // false dönersek pipeline bir sonraki handler'a geçer.
        if (exception is not NotFoundException) return false;

        httpContext.Response.StatusCode = StatusCodes.Status404NotFound;

        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "Not Found",
            Detail = exception.Message
        }, ct);

        return true;
    }
}