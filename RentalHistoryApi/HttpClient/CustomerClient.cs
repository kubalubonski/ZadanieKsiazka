using Newtonsoft.Json;
using RentalHistoryAPI.Models;
namespace RentalHistoryAPI;


public interface ICustomerClient
{
    Task<CustomerDto> GetCustomerById(int customerid);
}

public class CustomerClient: ICustomerClient
{
    private readonly HttpClient _httpClient;
    private readonly string _customerUrl = "https://localhost:7107/rentals/customers/only";
    private readonly ILogger<CustomerClient> _logger;
    private readonly IHttpContextAccessor _httpContextAccesor;


    public CustomerClient(HttpClient httpClient, ILogger<CustomerClient> logger, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpContextAccesor = httpContextAccessor;
    }

    public async Task<CustomerDto> GetCustomerById(int customerid)
    {

        //logi, X-request-ID
        var httpContext = _httpContextAccesor.HttpContext;
        var requestId = httpContext.Items["X-Request-ID"]?.ToString();
        if(!string.IsNullOrEmpty(requestId))
        {
            _httpClient.DefaultRequestHeaders.Add("X-Request-ID", requestId);
        }
        _logger.LogInformation($"Request action  with path: {_customerUrl}/{customerid} and customerId: {customerid}  GET_CUSTOMER_BY_ID action invoked");


        var response = await _httpClient.GetAsync($"{_customerUrl}/{customerid}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var customer = JsonConvert.DeserializeObject<CustomerDto>(content);

        _logger.LogInformation($"Request action  with path: {_customerUrl}/{customerid} and customerId: {customerid}  GET_CUSTOMER_BY_ID action executed with {response.EnsureSuccessStatusCode}");

        return customer;
    }
}