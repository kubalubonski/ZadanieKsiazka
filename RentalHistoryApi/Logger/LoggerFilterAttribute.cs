using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace RentalHistoryAPI.Filters;


public class LoggerFilterAttribbute: ActionFilterAttribute
{
    private readonly ILogger<LoggerFilterAttribbute> _logger;
    private Stopwatch _stopwatch;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public LoggerFilterAttribbute(ILogger<LoggerFilterAttribbute> logger, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        _stopwatch = Stopwatch.StartNew();

        var httpContext = _httpContextAccessor.HttpContext;
        
        if(!httpContext.Items.ContainsKey("X-Request-ID"))
        {
           var requestId = Guid.NewGuid().ToString();
           httpContext.Items["X-Request-ID"] = requestId;
           
        }

        _logger.LogInformation($"Request started: {context.HttpContext.Request.Method} {context.HttpContext.Request.Path} X-Request-ID: {httpContext.Items["X-Request-ID"]}");
       

    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        _stopwatch.Stop();
        var responseStatusCode = context.HttpContext.Response.StatusCode;
        var httpContext = _httpContextAccessor.HttpContext;
        var requestId = httpContext.Items["X-Request-ID"];


        _logger.LogInformation($"Request finished: {context.HttpContext.Request.Method} {context.HttpContext.Request.Path}, Response: {responseStatusCode}, Duration: {_stopwatch.ElapsedMilliseconds} ms, X-Request-ID: {requestId}");

        if (responseStatusCode >= 405)
        {
            var errorMessage = context.Exception?.ToString() ?? "(No additional error information)";
            _logger.LogError($"Request error: {context.HttpContext.Request.Method} {context.HttpContext.Request.Path}, Response: {responseStatusCode}, Error: {errorMessage}, X-Request-ID: {requestId}");
        }
    }
}