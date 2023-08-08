using KsiazkaAPI.Models;
using Microsoft.EntityFrameworkCore;
using KsiazkaAPI.Services;
using KsiazkaAPI;
using KsiazkaAPI.Middleware;
using NLog.Web;
using KsiazkaAPI.Filters;
using Microsoft.AspNetCore.Builder;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<BooksContext>(options => options.UseNpgsql(connectionString));
        // Add services to the container.


        builder.Services.AddScoped<ErrorHandlingMiddleware>();

        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddScoped<LoggerFilterAttribbute>();

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddHttpClient<RentalsClient>(client =>
    {
        client.BaseAddress = new Uri("https://localhost:7107/rentals"); // Adres URL serwera z książkami
    });

        builder.Services.AddScoped<IRentalsClient, RentalsClient>();

        builder.Services.AddScoped<IBookService, BookService>();

        //HttpClient
        builder.Services.AddHttpClient();

        // NLog: Setup NLog for Dependency injection
        builder.Logging.ClearProviders();
        builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
        builder.Host.UseNLog();

        NLogConfig.ConfigureNLog();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            
        }
        app.UseMiddleware<ErrorHandlingMiddleware>();
        app.UseHttpsRedirection();
        app.UseHttpLogging();

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookApiDocumentation");
        });
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}