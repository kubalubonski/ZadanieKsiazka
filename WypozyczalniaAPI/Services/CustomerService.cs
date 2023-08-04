using WypozyczalniaAPI.Entities;
using WypozyczalniaAPI;
using Microsoft.EntityFrameworkCore;
using Confluent.Kafka;
using Newtonsoft.Json;
using WypozyczalniaAPI.ModelsDto;

namespace WypozyczalniaAPI.Services;

public interface ICustomerService
{
    Task<Customer> Create(CustomerDto dto);
    Task<CustomerRentals> GetCustomer(int id);
    Task<List<CustomerRentals>> GetAllCustomers();
    Task<List<RentalDto>> GetRentalsForCustomer(int id);
    Task<bool> Update(int id, CustomerDto dto);
    Task<bool> Delete(int id);

}
public class CustomerService: ICustomerService
{


    private readonly RentalContext _dbContext;
    private readonly IBooksClient _client;
    private readonly ILogger<CustomerService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CustomerService(RentalContext dbContext, IBooksClient client, ILogger<CustomerService> logger, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _client = client;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Customer> Create(CustomerDto dto)
    {
        //Logging with X-Request-ID
        var requestId = _httpContextAccessor.HttpContext.Items["X-Request-ID"]?.ToString();
        _logger.LogInformation($"CREATE action for new rent invoked, X-Request-ID: {requestId}");

        var customer = dto.ToCustomer();
        var result = _dbContext.Customers.Add(customer);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"CREATE action for new rent executed, X-Request-ID: {requestId}");

        return result.Entity;
    }

    public async Task<CustomerRentals> GetCustomer(int id)
    {
        //Logging with X-Request-ID
        var requestId = _httpContextAccessor.HttpContext.Items["X-Request-ID"]?.ToString();
        _logger.LogInformation($"GET action for customer with id: {id}  invoked, X-Request-ID: {requestId}");

        var customer =  await _dbContext.Customers
                        
                        .FirstOrDefaultAsync(r => r.Customerid == id);

        if (customer is null) throw new NotFoundException("No such customer");

        
        var rentals = await GetRentalsForCustomer(id);
        
        var customerRentals = new CustomerRentals
        {
            Name = customer.Name,
            Surname = customer.Surname,
            Rentals = rentals
           
        };
        
        _logger.LogInformation($"GET action for rent with id: {id}  executed, X-Request-ID: {requestId}");

        return customerRentals;

    }
    public async Task<List<RentalDto>> GetRentalsForCustomer(int id)
    {
        var rentals = await _dbContext.Rentals.Where(r => r.Customerid == id)
                            .Select(r=> r.ToDto()).ToListAsync();
        
        return rentals;
    }

    public async Task<List<CustomerRentals>> GetAllCustomers()
    {
        //Logging with X-Request-ID
        var requestId = _httpContextAccessor.HttpContext.Items["X-Request-ID"]?.ToString();
        _logger.LogInformation($"GET action for all customers invoked, X-Request-ID: {requestId}");

        var customers = await _dbContext.Customers.ToListAsync();

        var customersWithRentals = new List<CustomerRentals>();

        foreach (var customer in customers)
        {
            var rentals = await GetRentalsForCustomer(customer.Customerid);

            var customerRentals = new CustomerRentals
            {
                Name = customer.Name,
                Surname = customer.Surname,
                Rentals = rentals
            };

            customersWithRentals.Add(customerRentals);
        }

        _logger.LogInformation($"GET action for all customers executed, X-Request-ID: {requestId}");

        return customersWithRentals;

    }

    public async Task<bool> Update(int id, CustomerDto dto)
    {
        //Logging with X-Request-ID
        var requestId = _httpContextAccessor.HttpContext.Items["X-Request-ID"]?.ToString();
        _logger.LogInformation($"Customer with id: {id} UPDATE action invoked, X-Request-ID: {requestId}");

        var customer = await _dbContext.Customers.FirstOrDefaultAsync(r => r.Customerid == id);

        if (customer is null)
        {
            throw new NotFoundException("No customer with this id");
        }

        customer.Name = dto.Name;
        customer.Surname = dto.Surname;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"Customer with id: {id} UPDATE action executed, X-Request-ID: {requestId}");

        return true;
    }

    public async Task<bool> Delete(int id)
    {
        //Logging with X-Request-ID
        var requestId = _httpContextAccessor.HttpContext.Items["X-Request-ID"]?.ToString();
        _logger.LogInformation($"Customer with id: {id} DELETE action invoked, X-Request-ID: {requestId}");

        var customer = await _dbContext.Customers.FirstOrDefaultAsync(r => r.Customerid == id);

        if(customer is null)
        {
            throw new NotFoundException("No customer with this id");
        }
        _dbContext.Customers.Remove(customer);

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"Customer with id: {id} DELETE action executed, X-Request-ID: {requestId}");

        return true;
    }

}