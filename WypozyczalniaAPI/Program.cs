using Microsoft.EntityFrameworkCore;
using WypozyczalniaAPI;
using WypozyczalniaAPI.Entities;
using WypozyczalniaAPI.Services;
using NLog.Web;
using WypozyczalniaAPI.Filters;
using Newtonsoft.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Formatters;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<RentalContext>(options => options.UseSqlite(connectionString));

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options => 
{ 
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});



builder.Services.AddScoped<ErrorHandlingMiddleware>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<LoggerFilterAttribbute>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient<BooksClient>(client =>
    {
        client.BaseAddress = new Uri("https://localhost:7008/books"); // Adres URL serwera z książkami
    });

builder.Services.AddScoped<IRentService, RentService>();

builder.Services.AddScoped<IBooksClient, BooksClient>();

builder.Services.AddScoped<ICustomerService, CustomerService>();

//builder.Services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddHttpContextAccessor();

//builder.Services.AddSession();




//HttpClient
builder.Services.AddHttpClient();

// NLog: Setup NLog for Dependency injection
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Host.UseNLog();

//Log
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

//app.UseSession();



app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "RentApiDocumentation");
        });

app.UseAuthorization();

app.MapControllers();

// using var scope = app.Services.CreateScope();
// var services = scope.ServiceProvider;
//  var context = services.GetRequiredService<RentalContext>();
//  await context.Database.MigrateAsync();
 
app.Run();

