using Microsoft.AspNetCore.Mvc;
using WypozyczalniaAPI.Entities;
using WypozyczalniaAPI.Services;
namespace WypozyczalniaAPI.Controllers;

using Microsoft.EntityFrameworkCore;
using WypozyczalniaAPI.Filters;
using WypozyczalniaAPI.ModelsDto;

[ApiController]
[Route("rentals/customers")]

[ServiceFilter(typeof(LoggerFilterAttribbute))]
public class CustomerController : ControllerBase
{
    
     private readonly RentalContext _dbContext;
     private readonly ICustomerService _customerService;
    
    public CustomerController(RentalContext dbContext, ICustomerService customerService)
    {
        _dbContext = dbContext;
        _customerService = customerService;
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] CustomerDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _customerService.Create(dto);

        return CreatedAtAction(nameof(Get), new {id = dto.ToCustomer().Customerid}, dto);;

    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerRentals>> Get([FromRoute]int id)
    {
        var customer = await _customerService.GetCustomer(id);

        return Ok(customer);
    }

    [HttpGet("{only}/{id}")]
    public async Task<ActionResult<CustomerDto>> GetOnlyCustomer([FromRoute] int id)
    {
        var customer = await _dbContext.Customers.FirstOrDefaultAsync(r => r.Customerid == id);
        var r  = customer.ToCustomerDto();
        
        
        return Ok(r);
    }

    [HttpGet]
    public async Task<ActionResult<List<CustomerRentals>>> Get()
    {
        var customers = await _customerService.GetAllCustomers();

        return Ok(customers);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CustomerDto>> Put([FromRoute] int id, [FromBody] CustomerDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var isUpdated = await _customerService.Update(id, dto);

        if (!isUpdated)
        {
            throw new NotFoundException();
        }

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<CustomerDto>> Delete([FromRoute] int id)
    {
        var customer = await _customerService.Delete(id);

        return NoContent();
    }

}