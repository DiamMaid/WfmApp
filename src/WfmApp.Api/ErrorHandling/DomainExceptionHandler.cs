using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WfmApp.Application.Common;

namespace WfmApp.Api.ErrorHandling;

public sealed class DomainExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, title) = exception switch
        {
            EntityNotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
            ArgumentException => (StatusCodes.Status422UnprocessableEntity, "Validation Failed"),
            InvalidOperationException => (StatusCodes.Status409Conflict, "Conflict"),
            _ => (0, string.Empty)
        };

        if (status == 0)
            return false;

        httpContext.Response.StatusCode = status;

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = exception.Message
        };

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }
}
