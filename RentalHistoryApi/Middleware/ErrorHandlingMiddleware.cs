namespace RentalHistoryAPI.Middleware;
using RentalHistoryAPI.Exceptions;

public class ErrorHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
    {
        _logger = logger;
    }
    async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next.Invoke(context);
        }
        catch(NotFoundException notFoundException)
        {
            _logger.LogError(notFoundException.Message);
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync(notFoundException.Message);
        }
        catch(BadRequestException badRequestException)
        {
            _logger.LogError(badRequestException.Message);
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync(badRequestException.Message);
        }
        // catch (Exception e)
        // {
        //     _logger.LogError(e, e.Message);
        //     context.Response.StatusCode = 500;
        //     await context.Response.WriteAsync("Something went wrong");
        // }
    }
}
