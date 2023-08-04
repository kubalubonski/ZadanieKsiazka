using WypozyczalniaAPI.Entities;
using WypozyczalniaAPI;
using Microsoft.EntityFrameworkCore;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace WypozyczalniaAPI.Services;


public interface IRentService
{
    Task Create(RentalDto dto);
    Task<RentWithBookDto> GetRentWithBook(int id);
    Task<List<RentWithBookDto>> GetRentalsWithBook(DateTime? from, DateTime? to);
    Task<bool> Update(int id, RentalDto dto);

    Task<bool> Delete(int id);
    Task<List<RentalDto>> GetRentalsByBookId(int bookId);
    Task SendRentalEventCreateToKafka(int id, string action);
    //Task<RentalEvent> SendRentalEventCreateToKafka(RentalDto dto, string action);
}
public class RentService : IRentService
{
    private readonly RentalContext _dbContext;

    private readonly IBooksClient _client;
    private readonly ILogger<RentService> _logger;
    private readonly string _kafkaTopic = "rentalEventsNew";
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    

    public RentService(RentalContext dbContext, IBooksClient client, ILogger<RentService> logger, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _client = client;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<RentWithBookDto>> GetRentalsWithBook(DateTime? from, DateTime? to)
    {
        
        //Logging with X-Request-ID
        var requestId = _httpContextAccessor.HttpContext.Items["X-Request-ID"]?.ToString();
        _logger.LogInformation($"GET action for all rents with data filtering parameters: {from} and {to} invoked, , X-Request-ID: {requestId}");
        

        var query = _dbContext.Rentals.AsQueryable();
        
        if (from.HasValue)
        {
            query = query.Where(d => d.RentDate >= from.Value);
        }
        
        if (to.HasValue)
        {
            query = query.Where(d => d.RentDate <= to.Value);
        }
        
        var rentals = await query.ToListAsync();

        var rentalWithBooks = new List<RentWithBookDto>();

        foreach (var rental in rentals)
        {
            var book = await _client.GetBookById(rental.Bookid);

            var rentalWithBook = new RentWithBookDto
            {
                Name = rental.Name,
                Surname = rental.Surname,
                RentDate = rental.RentDate,
                Customerid = rental.Customerid,
                Bookid = rental.Bookid,
                BookInfo = book
            };
            rentalWithBooks.Add(rentalWithBook);
        }
        
        _logger.LogInformation($"GET action for all rents with data filtering executed, , X-Request-ID: {requestId}");
        
        return rentalWithBooks;
        
    }
    
    public async Task<RentWithBookDto> GetRentWithBook(int id)
    {
        
        //Logging with X-Request-ID
        var requestId = _httpContextAccessor.HttpContext.Items["X-Request-ID"]?.ToString();
        _logger.LogInformation($"GET action for rent with id: {id}  invoked, , X-Request-ID: {requestId}");
        
        var rental = await _dbContext.Rentals
                    .FindAsync(id);

        if(rental is null)
        {
            throw new  NotFoundException("No rent with this id, ");
        }

        var book = await _client.GetBookById(rental.Bookid);

        if (book is null)
        {
            throw new NotFoundException("No book with this rent");
        }
        
        var rentalWithBook = new RentWithBookDto()
        {
            Name = rental.Name,
            Surname = rental.Surname,
            RentDate = rental.RentDate,
            Customerid = rental.Customerid,
            Bookid = rental.Bookid,
            BookInfo = book
        };
         
        _logger.LogInformation($"GET action for rent with id: {id}  executed, , X-Request-ID: {requestId}");
        
        return rentalWithBook;
    }

    public async Task<List<RentalDto>> GetRentalsByBookId(int bookId)
    {
        var rentals = await _dbContext.Rentals.Where(r => r.Bookid == bookId)
                            .Select(r=> r.ToDto()).ToListAsync();

        if(rentals != null) 
            {
                return rentals;
            }
        throw new NotFoundException("No rent with this bookId");
    }

    public async Task Create(RentalDto dto)
    {
        
        //Logging with X-Request-ID
        var requestId = _httpContextAccessor.HttpContext.Items["X-Request-ID"]?.ToString();
        _logger.LogInformation($"CREATE action for new rent invoked, , X-Request-ID: {requestId}");


        var rent = dto.ToRental();
        
        _dbContext.Rentals.Add(rent);

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"CREATE action for new rent executed, , X-Request-ID: {requestId}");

        //To kafka
        await SendRentalEventCreateToKafka(rent.Rentid, "Created");
    }

    

    public async Task<bool> Update(int id, RentalDto dto)
    {
        
        //Logging with X-Request-ID
        var requestId = _httpContextAccessor.HttpContext.Items["X-Request-ID"]?.ToString();
        _logger.LogInformation($"Rent with id: {id} UPDATE action invoked, , X-Request-ID: {requestId}");


        var rent = await _dbContext.Rentals.FirstOrDefaultAsync(r => r.Rentid == id);
        
        if (rent is null)
        {
            throw new NotFoundException("No rent with this id");
        }
        rent.Name = dto.Name;
        rent.Surname = dto.Surname;
        rent.RentDate = dto.RentDate;
        rent.Bookid = dto.Bookid;
        
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"Book with id: {id} UPDATE action executed");

        return true;
    }

    public async Task<bool> Delete(int id)
    {

       //Logging with X-Request-ID
        var requestId = _httpContextAccessor.HttpContext.Items["X-Request-ID"]?.ToString();
        _logger.LogInformation($"Rent with id: {id} DELETE action invoked, , X-Request-ID: {requestId}");


        
        var rent = await _dbContext.Rentals.FirstOrDefaultAsync(r => r.Rentid == id);
        
        if (rent is null)
        {
            throw new NotFoundException("No rent with this id");
        }
        //To kafka
        await SendRentalEventDeleteToKafka(id, "Deleted");
        
        _dbContext.Rentals.Remove(rent);
        
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"Rent with id: {id} DELETE action executed, , X-Request-ID: {requestId}");
        
        
        

        return true;

        
    }
    //Kafka
    public async Task SendRentalEventCreateToKafka(int id, string action)
    {
       
        var config = new ProducerConfig {BootstrapServers = "localhost:9092"};
        
        using (var producer = new ProducerBuilder<Null, string>(config).Build())
        {
            try 
            {
                
                var rental = await _dbContext.Rentals.FirstOrDefaultAsync(r => r.Rentid == id);
               
                //var book = await _client.GetBookById(rental.Bookid);
                
                var rentalEvent = new RentalEvent
                {
                    Name = rental.Name,
                    Rentid = rental.Rentid, 
                    Customerid = rental.Customerid,
                    Bookid = rental.Bookid,
                    RentDate = rental.RentDate,
                    ReturnDate = null
                    
                       
                };
               
                
                var rentalJson = JsonConvert.SerializeObject(rentalEvent);
                var message = new Message<Null, string> {Value = rentalJson};

                var report = await producer.ProduceAsync(_kafkaTopic, message);


                //Logging with X-Request-ID
                var requestId = _httpContextAccessor.HttpContext.Items["X-Request-ID"]?.ToString();
                _logger.LogInformation($"Kafka message(create rent) sent (topic: {report.Topic}, partition: {report.Partition}, offset: {report.Offset}), , X-Request-ID: {requestId}");

            }
            catch (ProduceException<Null, string>)
            {
                _logger.LogError($"Kafka message delivery failed");
            }
           
        
        }
        
        
    }
    public async Task SendRentalEventDeleteToKafka(int id, string action)
    {
        var config = new ProducerConfig {BootstrapServers = "localhost:9092"};
        

        using (var producer = new ProducerBuilder<Null, string>(config).Build())
        {
            try 
            {
                
                var rental = await _dbContext.Rentals.FirstOrDefaultAsync(r => r.Rentid == id);
               
                //var book = await _client.GetBookById(rental.Bookid);
                
                var rentalEvent = new RentalEvent
                {
                    Name = rental.Name,
                    Rentid = rental.Rentid, 
                    Customerid = rental.Customerid,
                    Bookid = rental.Bookid,
                    RentDate = rental.RentDate,
                    ReturnDate = DateTime.Now.Date
                    
                       
                };
               
                
                var rentalJson = JsonConvert.SerializeObject(rentalEvent);
                var message = new Message<Null, string> {Value = rentalJson};

                var report = await producer.ProduceAsync(_kafkaTopic, message);

                
                //Logging with X-Request-ID
                var requestId = _httpContextAccessor.HttpContext.Items["X-Request-ID"]?.ToString();
                _logger.LogInformation($"Kafka message(delete rent) sent (topic: {report.Topic}, partition: {report.Partition}, offset: {report.Offset}), X-Request-ID: {requestId}");

            }
            catch (ProduceException<Null, string>)
            {
                _logger.LogError($"Kafka message delivery failed");
            }
           
        
        }
        
        
    }


}