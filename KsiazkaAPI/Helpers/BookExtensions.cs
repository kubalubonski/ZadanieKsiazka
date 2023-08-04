
using KsiazkaAPI.Models;
using Microsoft.EntityFrameworkCore;
namespace KsiazkaAPI.Helpers;

public static class BookExtensions
{
    public static BookDto ToDto(this Book book)
    {

        if (book != null)
        {
           
            return new BookDto
            {
                Title = book.Title,
                Author = book.Author,
                ReleaseDate = book.ReleaseDate.ToDateTime(TimeOnly.MinValue)
                
    
            };
        }
        return null;
        
}
    public static Book ToBook(this BookDto dto)
    {
        if (dto != null)
        {
            return new Book
            {
                Title = dto.Title,
                Author = dto.Author,
                ReleaseDate = DateOnly.FromDateTime(dto.ReleaseDate)
                
            };
        }
        return null;
    }
}
