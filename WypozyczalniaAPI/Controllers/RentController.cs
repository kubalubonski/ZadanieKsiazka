using Microsoft.AspNetCore.Mvc;
using WypozyczalniaAPI.Entities;
using WypozyczalniaAPI.Services;
namespace WypozyczalniaAPI.Controllers;
using WypozyczalniaAPI.Filters;
using WypozyczalniaAPI.ModelsDto;

[ApiController]
[Route("rentals")]

[ServiceFilter(typeof(LoggerFilterAttribbute))]
public class RentController : ControllerBase
{
    private readonly IRentService _rentService;
     private readonly RentalContext _dbContext;
    
    public RentController(IRentService rentService, RentalContext dbContext)
    {
        _dbContext = dbContext;
        _rentService = rentService;
    }
    

    [HttpPost]
    public  async Task<ActionResult> Post([FromBody] RentalDto dto)
    {
       if(!ModelState.IsValid)
       {
            throw new BadRequestException("Wrong data");
       }
       await _rentService.Create(dto);

       return CreatedAtAction(nameof(Get), new{id = dto.ToRental().Rentid}, dto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RentWithBookDto>> Get([FromRoute] int id)
    {
        var rent = await _rentService.GetRentWithBook(id);

        return Ok(rent);
    }
    [HttpGet]
    public async Task<ActionResult<List<RentWithBookDto>>> Get([FromQuery]DateTime? from, [FromQuery]DateTime? to)
    {
        var rentals = await _rentService.GetRentalsWithBook(from, to);

        return Ok(rentals);
    }

    [HttpGet("books/{bookId}")]
    public async Task<ActionResult<List<RentWithBookDto>>> GetB(int bookId)
    {
        var rentals = await _rentService.GetRentalsByBookId(bookId);
        
        if (rentals != null)
        {
            return Ok(rentals);
        }
        throw new NotFoundException("There is no rent with this book");
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<RentalDto>> Put([FromRoute] int id,[FromBody] RentalDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
          //throw new BadRequestException("Model is not validated");
        }

        var isUpdated = await _rentService.Update(id, dto);

        if(!isUpdated)
        {
            throw new NotFoundException();
        }
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<RentalDto>> Delete([FromRoute] int id)
    {
        var rent = await _rentService.Delete(id);
        
         
        return NoContent();
    }

    


}