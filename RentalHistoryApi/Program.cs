using Confluent.Kafka;
using NLog.Web;
using RentalHistoryAPI;
using RentalHistoryAPI.Services;
using System.Text.Json.Serialization;
using RentalHistoryAPI.Filters;
using RentalHistoryAPI.Middleware;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);


 //mongoDB
//  builder.Services.AddSingleton<IMongoClient>(
//     sp => 
//     {
//         var settings = sp.GetRequiredService<IConfiguration>().GetSection("RentalHistoryDatabase");
//         var connectionString = settings["ConnectionString"];
//                         return new MongoClient(connectionString);
//     }
//  );
builder.Services.Configure<RentalHistoryDatabaseSettings>(

    builder.Configuration.GetSection("RentalHistoryDatabase")
);



// Add services to the container.

var _kafkaConfig  = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "lalalala"
        };

builder.Services.AddSingleton<ConsumerConfig>(_kafkaConfig);
builder.Services.AddHostedService<KafkaMessageHandlerService>();

builder.Services.AddScoped<ErrorHandlingMiddleware>();

builder.Services.AddScoped<LoggerFilterAttribbute>();

builder.Services.AddScoped<IRentalHistoryService, RentalHistoryService>();
builder.Services.AddHttpContextAccessor();



//HttpCLient
builder.Services.AddHttpClient();
builder.Services.AddScoped<ICustomerClient, CustomerClient>();
builder.Services.AddScoped<IBooksClient, BooksClient>();

builder.Services.AddControllers()
    .AddJsonOptions(
        options => 
        {options.JsonSerializerOptions.PropertyNamingPolicy = null;
        
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });




builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "RentalHistoryApiDocumentation");
        });

app.UseAuthorization();

app.MapControllers();

app.Run();
