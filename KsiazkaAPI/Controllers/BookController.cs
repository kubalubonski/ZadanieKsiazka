using Microsoft.AspNetCore.Mvc;
using KsiazkaAPI;
using KsiazkaAPI.Models;
using Microsoft.EntityFrameworkCore;
using KsiazkaAPI.Services;
using KsiazkaAPI.Filters;


using KsiazkaAPI.Helpers;
namespace KsiazkaAPI.Controllers;
using KsiazkaAPI.Exceptions;
using KsiazkaAPI.Filters;


[ApiController]
[Route("books")]

[ServiceFilter(typeof(LoggerFilterAttribbute))]
public class BookController : ControllerBase
{
    private readonly IBookService _bookService;
    private readonly ILogger<LoggerFilterAttribbute> _logger;
    
    public BookController(IBookService bookService, ILogger<LoggerFilterAttribbute> logger)
    {
        _bookService = bookService;
        _logger = logger;
    }

    [HttpPost]
    
    public  async Task<ActionResult> PostBook([FromBody] BookDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await _bookService.Create(dto);
        
        return CreatedAtAction(nameof(Get), new {id = dto.ToBook().Bookid}, dto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDto>>> Get([FromQuery]string title = "", [FromQuery]string author = "")
    {
        var books = await _bookService.GetBooks(title, author);

        return Ok(books);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<BookDto>> Get([FromRoute] int id)
    {
        var book = await  _bookService.Get(id);   
        
        return Ok(book);
    }


    [HttpPut("{id}")]
    public async Task<ActionResult<BookDto>> Put([FromRoute] int id,[FromBody] BookDto dto)
    {

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var isUpdated = await _bookService.Update(id, dto);

        if (!isUpdated)
        {
            throw new NotFoundException();
        }
        
        return Ok();

    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<BookDto>> Delete([FromRoute] int id)
    {
        var book =  await _bookService.Delete(id);
        
        return NoContent();
    }    
    
    [HttpGet("test")]
        public async Task<ActionResult<string>> Test()
        {
            return Ok("test");
        }

}

