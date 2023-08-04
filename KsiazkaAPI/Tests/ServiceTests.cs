using KsiazkaAPI.Services;
using KsiazkaAPI.Controllers;
using KsiazkaAPI.Helpers;
using KsiazkaAPI.Models;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KsiazkaAPI.Exceptions;

namespace KsiazkaAPI;

public class ServiceTests 
{
    [Fact]
    public async Task Get_Test()
    {
        var bookId = 1;
        var dbContextOptions = new DbContextOptionsBuilder<BooksContext>()
                                    .UseInMemoryDatabase(databaseName: "Test_Books").Options;

        using (var dbContext = new BooksContext(dbContextOptions))
        {
            var book = new Book {Bookid = bookId, Title = "ławka", Author = "widelec", ReleaseDate = new DateOnly(2010,3,4)};
            dbContext.Books.Add(book);
            dbContext.SaveChanges();

            var mockLogger = new Mock<ILogger<BookService>>();
            var mockHttpAccessor = new Mock<IHttpContextAccessor>();
            var service = new BookService(dbContext, mockLogger.Object, null, mockHttpAccessor.Object);

            var result = await service.Get(bookId);

            Assert.NotNull(result);
            Assert.Equal(book.Title, result.Title);
            Assert.Equal(book.Author, result.Author);
            Assert.Equal(book.ToDto().ReleaseDate, result.ReleaseDate);
        }

    }


    [Fact]
    public async Task Create_Test()
    {
        var bookId = 1;
        
        var book = new Book {Bookid = bookId, Title = "ławka", Author = "widelec",
                             ReleaseDate = new DateOnly(2010,3,4)};
       
        var dbContextOptions = new DbContextOptionsBuilder<BooksContext>()
                                    .UseInMemoryDatabase(databaseName: "Test_Books2").Options;

        using(var dbContext = new BooksContext(dbContextOptions))
        {
            var mockLogger = new Mock<ILogger<BookService>>();
            var mockHttpAccessor = new Mock<IHttpContextAccessor>();
            var service = new BookService(dbContext, mockLogger.Object,null, mockHttpAccessor.Object);

            var result = await service.Create(book.ToDto());

            Assert.Equal(1, dbContext.Books.Count());
            Assert.Equal(book.Bookid, result.Bookid);
            Assert.Equal(book.Author, result.Author);
            Assert.Equal(book.Title, result.Title);
            Assert.Equal(book.ReleaseDate, result.ReleaseDate);
        }    
    }

    [Fact]
    public async Task Create_BookExists_Test()
    {
        var bookId = 1;
        var bookId2 = 2;
        var book = new Book {Bookid = bookId, Title = "ławka", Author = "widelec",
                             ReleaseDate = new DateOnly(2010,3,4)};
        var existingBook = new Book {Bookid = bookId2, Title = "ławka", Author = "widelec",
                             ReleaseDate = new DateOnly(2010,3,4)};                     

        var dbContextOptions = new DbContextOptionsBuilder<BooksContext>()
                                    .UseInMemoryDatabase(databaseName: "Test_Books3").Options;

        using (var dbContext = new BooksContext(dbContextOptions))
        {
            dbContext.Add(existingBook);
            dbContext.SaveChanges();

            var mockLogger = new Mock<ILogger<BookService>>();
            var mockHttpAccessor = new Mock<IHttpContextAccessor>();
            var service = new BookService(dbContext, mockLogger.Object, null, mockHttpAccessor.Object);

            await Assert.ThrowsAsync<ConflictException>(() => service.Create(book.ToDto()));
        }                     
    }

    [Fact]
    public async Task Update_Test()
    {
        var bookId = 1;
        var previusTitle = "Monk";
        var newTitle = "Malik";

        var dbContextOptions = new DbContextOptionsBuilder<BooksContext>()
                                    .UseInMemoryDatabase(databaseName: "Test_Books4").Options;

        using (var dbContext = new BooksContext(dbContextOptions))
        {
            var book = new Book { Bookid = bookId, Title = previusTitle, Author = "Polska", ReleaseDate= new DateOnly(2022,1,1)};
            dbContext.Books.Add(book);
            
            await dbContext.SaveChangesAsync();
            var mockLogger = new Mock<ILogger<BookService>>();
            var mockHttpAccessor = new Mock<IHttpContextAccessor>();
            var service = new BookService(dbContext, mockLogger.Object, null, mockHttpAccessor.Object);
            var dto = new BookDto {Title = newTitle, Author = "Polska", ReleaseDate= new DateTime(2022,1,1)};

            var result = await service.Update(bookId, dto);

            Assert.True(result);
            var updatedBook = await dbContext.Books.FirstOrDefaultAsync(b => b.Bookid == bookId);
            Assert.NotNull(updatedBook);
            Assert.Equal(newTitle, updatedBook.Title);

        }
    }

    [Fact]
    public async Task Delete_WithRent_Test()
    {
        var bookId = 1;
        var dbContextOptions = new DbContextOptionsBuilder<BooksContext>()
                                    .UseInMemoryDatabase(databaseName: "Test_Books5").Options;
        
        using (var dbContext = new BooksContext(dbContextOptions))
        {
            var book = new Book {Bookid = bookId, Title = "ławka", Author = "widelec", ReleaseDate = new DateOnly(2010,3,4)};
            dbContext.Books.Add(book);
            await dbContext.SaveChangesAsync();

            var mockLogger = new Mock<ILogger<BookService>>();
            var mockRentalsClient = new Mock<IRentalsClient>();
            var mockHttpAccessor = new Mock<IHttpContextAccessor>();
            mockRentalsClient.Setup(client => client.DoesRentalExist(bookId)).ReturnsAsync(true);

            var service = new BookService(dbContext, mockLogger.Object, mockRentalsClient.Object, mockHttpAccessor.Object);

            //var result = await service.Delete(bookId);

            await Assert.ThrowsAsync<NotFoundException>(() => service.Delete(bookId));
            Assert.Equal(1, dbContext.Books.Count());
        }
    }

    [Fact]
    public async Task Delete_WithoutRent_Test()
    {
        var bookId = 1;
        var dbContextOptions = new DbContextOptionsBuilder<BooksContext>()
                                    .UseInMemoryDatabase(databaseName: "Test_Books6").Options;
        


        using (var dbContext = new BooksContext(dbContextOptions))
        {
            var book = new Book {Bookid = bookId, Title = "ławka", Author = "widelec", ReleaseDate = new DateOnly(2010,3,4)};
            dbContext.Books.Add(book);
            await dbContext.SaveChangesAsync();

            var mockLogger = new Mock<ILogger<BookService>>();
            var mockRentalsClient = new Mock<IRentalsClient>();
            var mockHttpAccessor = new Mock<IHttpContextAccessor>();
            mockRentalsClient.Setup(client => client.DoesRentalExist(bookId)).ReturnsAsync(false);

            var service = new BookService(dbContext, mockLogger.Object, mockRentalsClient.Object, mockHttpAccessor.Object);

            var result = await service.Delete(bookId);

            Assert.True(result);
            Assert.Equal(0, dbContext.Books.Count());
        }

    }
}