using Newtonsoft.Json;
using System.Net.Http.Headers;
namespace KsiazkaAPI;

public interface IRentalsClient
{
    Task<bool> DoesRentalExist(int bookId);
}
public class RentalsClient: IRentalsClient
{
    private readonly HttpClient _httpClient;
    private readonly string _rentalUrl = "https://localhost:7107/rentals/books";
    private readonly ILogger<RentalsClient> _logger;
    private readonly IHttpContextAccessor _httpContextAccesor;

    public RentalsClient(HttpClient httpClient, ILogger<RentalsClient> logger, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpContextAccesor = httpContextAccessor;
    }

    public async Task<bool> DoesRentalExist(int bookId)
    {

        //logi, X-request-ID
        var httpContext = _httpContextAccesor.HttpContext;
        var requestId = httpContext.Items["X-Request-ID"]?.ToString();

        if(!string.IsNullOrEmpty(requestId))
        {
            _httpClient.DefaultRequestHeaders.Add("X-Request-ID", requestId);
        }
        _logger.LogInformation($"Request action  with path: {_rentalUrl}/{bookId} and bookId: {bookId}  DOESRENTALEXIST action invoked");
        
        var response = await _httpClient.GetAsync($"{_rentalUrl}/{bookId}");

        _logger.LogInformation($"Request action  with path: {_rentalUrl}/{bookId} and bookId: {bookId}  DOESRENTALEXIST action executed with response {response.IsSuccessStatusCode}");

        return response.IsSuccessStatusCode;

    }
}