using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RentalHistoryAPI.Models;
using RentalHistoryAPI.Helpers;

namespace RentalHistoryAPI.Services;

public interface IRentalHistoryService
{
    Task<List<RentalData>> GetC();
    Task<CustomerHistory> GetCustomersHistory(int customerid, int n);
}
public class RentalHistoryService: IRentalHistoryService
{
    private readonly IMongoCollection<RentalData> _rentalHistoryCollection;
    private readonly ICustomerClient _client;
    private readonly IBooksClient _Bclient;
    private readonly ILogger<RentalHistoryService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RentalHistoryService(IOptions<RentalHistoryDatabaseSettings> databaseSettings, ICustomerClient client, IBooksClient Bclient, ILogger<RentalHistoryService> logger, IHttpContextAccessor httpContextAccessor)
    {
       _client = client;
       _Bclient = Bclient;
       _logger = logger;
       _httpContextAccessor = httpContextAccessor;

       var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
       var mongoBase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
       _rentalHistoryCollection = mongoBase.GetCollection<RentalData>(databaseSettings.Value.RentalHistoryCollectionName); 
       
    }
    public async Task<List<RentalData>> GetC()
    {
        //Logging with X-Request-ID
        var requestId = _httpContextAccessor.HttpContext.Items["X-Request-ID"]?.ToString();
        _logger.LogInformation($"GET action for all customers invoked, , X-Request-ID: {requestId}");


        var rentals =  await _rentalHistoryCollection.Find(_ => true).ToListAsync();

        _logger.LogInformation($"GET action for all customers executed, , X-Request-ID: {requestId}");

        return rentals;
    }

    public async Task<CustomerHistory> GetCustomersHistory(int customerid, int n)
    {
        //Logging with X-Request-ID
        var requestId = _httpContextAccessor.HttpContext.Items["X-Request-ID"]?.ToString();
        _logger.LogInformation($"GET action to see customer with id: {customerid} invoked, X-Request-ID: {requestId}");


        

        var customerDto = await _client.GetCustomerById(customerid);

        var rentalsHistory = await _rentalHistoryCollection
                                    .Find(r => r.Customerid == customerid && r.ReturnDate != null)
                                    .SortByDescending(r=> r.ReturnDate)
                                    .Limit(n)
                                    .ToListAsync();
        
        var rentalList = new List<RentalDataDto>();

        foreach (var rentalHistory in rentalsHistory)
        {
            var book = await _Bclient.GetBookById(rentalHistory.Bookid);
            
            var rentalWithBook = new RentalDataDto
            {
                Bookid = rentalHistory.Bookid,
                Title = book.Title,
                RentDate = rentalHistory.RentDate,
                ReturnDate = rentalHistory.ReturnDate
            };
            rentalList.Add(rentalWithBook);
        }

        var customerHistory = new CustomerHistory
        {
            customer = customerDto,
            rentalHistory = rentalList
        };

        _logger.LogInformation($"GET action to see customer with id: {customerid} executed, X-Request-ID: {requestId}");

        return customerHistory;
    }


}