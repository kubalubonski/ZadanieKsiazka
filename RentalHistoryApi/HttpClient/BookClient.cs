namespace RentalHistoryAPI;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using RentalHistoryAPI.Models;



public interface IBooksClient
{
    Task<BookDto> GetBookById(int bookId);
}
public class BooksClient : IBooksClient
{
    private readonly HttpClient _httpClient;
    private readonly string _bookUrl = "https://localhost:7008/books";
    private readonly ILogger<BooksClient> _logger;
    private readonly IHttpContextAccessor _httpContextAccesor;
    
    public BooksClient(HttpClient httpClient, ILogger<BooksClient> logger, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpContextAccesor = httpContextAccessor;
    }

    public async Task<BookDto> GetBookById(int bookId)
    {

        //logi, X-request-ID
        var httpContext = _httpContextAccesor.HttpContext;
        var requestId = httpContext.Items["X-Request-ID"]?.ToString();
        if(!string.IsNullOrEmpty(requestId))
        {
            _httpClient.DefaultRequestHeaders.Add("X-Request-ID", requestId);
        }
       _logger.LogInformation($"Request action  with path: {_bookUrl}/{bookId} and bookId: {bookId}  GET_BOOK_BY_ID action invoked");


        var response = await _httpClient.GetAsync($"{_bookUrl}/{bookId}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var book = JsonConvert.DeserializeObject<BookDto>(content);
        
        _logger.LogInformation($"Request action  with path: {_bookUrl}/{bookId} and bookId: {bookId}  GET_BOOK_BY_ID action executed with {response.EnsureSuccessStatusCode}");
        
        return book;
    }

}