using RentalHistoryAPI.Services;
using RentalHistoryAPI.Exceptions;
using Microsoft.AspNetCore.Mvc;
using RentalHistoryAPI.Models;
using RentalHistoryAPI.Filters;

namespace RentalHistoryAPI.Controllers;

[ApiController]
[Route("rentals")]

[ServiceFilter(typeof(LoggerFilterAttribbute))]
public class RentalHistoryController : ControllerBase
{
    private readonly IRentalHistoryService _historyService;

    public RentalHistoryController(IRentalHistoryService historyService)
    {
        _historyService = historyService;

    }

    [HttpGet]
    public async Task<ActionResult<List<RentalData>>> Get()
    {
        var rental = await _historyService.GetC();
        
        return Ok(rental);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDto>> GetCustomer([FromRoute] int id)
    {
            var customersHistory = await _historyService.GetCustomersHistory(id, 10);
            

            if (customersHistory is null)
            {
                throw new NotFoundException("No such customer");
            }
            return Ok(customersHistory);
    }

}