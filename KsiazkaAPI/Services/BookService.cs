using KsiazkaAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KsiazkaAPI.Controllers;
using KsiazkaAPI.Helpers;
using KsiazkaAPI.Exceptions;


namespace KsiazkaAPI.Services;

public interface IBookService
{
    
    Task<bool> Delete(int id);
    Task<BookDto> Get(int id);
    Task<IEnumerable<BookDto>> GetBooks(string title, string author);
    Task<bool> Update(int id, BookDto dto);
    
    Task<Book> Create(BookDto dto);
}
public class BookService : IBookService
{
    private readonly BooksContext _dbContext;
    private readonly ILogger<BookService> _logger;
    private readonly IRentalsClient _client;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BookService(BooksContext dbContext, ILogger<BookService> logger, IRentalsClient client, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _logger = logger;
        _client = client;
        _httpContextAccessor = httpContextAccessor;
    }

    

    public async Task<BookDto> Get(int id)
    {

        //Logging with X-Request-ID
        var requestId = _httpContextAccessor.HttpContext.Items["X-Request-ID"]?.ToString();
        _logger.LogInformation($"Book with id: {id} GET action invoked, X-Request-ID: {requestId}");

        var book = await _dbContext.Books
                    .FirstOrDefaultAsync(r => r.Bookid == id);

        if (book is null)
        {
            throw new NotFoundException("No book with this id");
        }

        _logger.LogInformation($"Book with id: {id} GET action executed, X-Request-ID: {requestId}");

        return book.ToDto();
    }
    public async Task<IEnumerable<BookDto>> GetBooks(string title, string author)
    {

        //Logging with X-Request-ID
        var requestId = _httpContextAccessor.HttpContext.Items["X-Request-ID"]?.ToString();
        _logger.LogInformation($"GET action for all books with filtering parameters: {title} and {author} invoked, X-Request-ID: {requestId}");
        
        var query =  _dbContext.Books.AsQueryable();

        if (!string.IsNullOrEmpty(title))
        {
            query = query.Where(t => t.Title.Contains(title));
        }
        if (!string.IsNullOrEmpty(author))
        {
                query = query.Where(a => a.Author.Contains(author)); 
        }
        
        var chosenBooks = await query.Select(r => r.ToDto()).ToListAsync();

        _logger.LogInformation($"GET action for all books with filtering executed, X-Request-ID: {requestId}");

        return chosenBooks;
    }  
    

    public async Task<Book> Create(BookDto dto)
    {

        //Logging with X-Request-ID
        var requestId = _httpContextAccessor.HttpContext.Items["X-Request-ID"]?.ToString();
        _logger.LogInformation($"CREATE action for new book invoked, X-Request-ID: {requestId}");

        if(await DoesBookExist(dto.Title, dto.Author))
        {
            throw new ConflictException("A book with the same author and title already exists.");
        }
        
        var book = dto.ToBook();
        var result = _dbContext.Books.Add(book);
        
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"CREATE action for new book executed, X-Request-ID: {requestId}");

        return result.Entity;
        

    }
    public async Task<bool> DoesBookExist(string title, string author)
    {

        //Logging with X-Request-ID
        var requestId = _httpContextAccessor.HttpContext.Items["X-Request-ID"]?.ToString();
        _logger.LogInformation($"{DoesBookExist} method with {title} and {author} parametres action invoked, X-Request-ID: {requestId}");
        
        var existingBook = await _dbContext.Books.FirstOrDefaultAsync(b => b.Title == title && b.Author == author);
        
        if(existingBook != null)
        {
            _logger.LogInformation($"{DoesBookExist} method with {title} and {author} parametres action executed with {true}, X-Request-ID: {requestId}");
            
            return true;
        }
        
        _logger.LogInformation($"{DoesBookExist} method with {title} and {author} parametres action executed with {false}, X-Request-ID: {requestId}");
        
        return false;
        
    }
    public async Task<bool> Update(int id, BookDto dto)
    {

        //Logging with X-Request-ID
        var requestId = _httpContextAccessor.HttpContext.Items["X-Request-ID"]?.ToString();
         _logger.LogInformation($"Book with id: {id} UPDATE action invoked, X-Request-ID: {requestId}");
        
        var book = await _dbContext.Books.FirstOrDefaultAsync(r => r.Bookid == id);

        if (book is null)
        {
            throw new NotFoundException("No book with this id");
        }

        book.Title = dto.Title;
        book.Author = dto.Author;
        book.ReleaseDate = DateOnly.FromDateTime(dto.ReleaseDate);
        
        await _dbContext.SaveChangesAsync();

         _logger.LogInformation($"Book with id: {id} UPDATE action executed, X-Request-ID: {requestId}");

        return true;

    }

    public async Task<bool> Delete(int id)
    {

        //Logging with X-Request-ID
        var requestId = _httpContextAccessor.HttpContext.Items["X-Request-ID"]?.ToString();
        _logger.LogInformation($"Book with id: {id} DELETE action invoked, X-Request-ID: {requestId}");
        
        var book = _dbContext
                    .Books
                    .FirstOrDefault(r => r.Bookid == id);

        if (book is null)
        {
            throw new NotFoundException("No book with this id");
        }
        
        var rental = await _client.DoesRentalExist(id);
        
        if (!rental)
        {
            _dbContext.Books.Remove(book);
            await _dbContext.SaveChangesAsync();
            
            _logger.LogInformation($"Book with id: {id} DELETE action executed, X-Request-ID: {requestId}");
            
            return true;
        }
        
        throw new NotFoundException("There is rental for this book. You cannot delete it");
    
    }

    
}