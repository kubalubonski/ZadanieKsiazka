
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using RentalHistoryAPI;
using RentalHistoryAPI.Models;



namespace RentalHistoryAPI.Services;

public class KafkaMessageHandlerService : BackgroundService
{
    private readonly IMongoCollection<RentalData> _rentalHistoryCollection;
    private readonly string _kafkaTopic = "rentalEventsNew";
    private readonly ConsumerConfig _kafkaConfig;
    private readonly ILogger<KafkaMessageHandlerService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    

    public KafkaMessageHandlerService(IOptions<RentalHistoryDatabaseSettings> databaseSettings, ConsumerConfig kafkaConfig, ILogger<KafkaMessageHandlerService> logger, IHttpContextAccessor httpContextAccessor)
    {
       
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;

        var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
        var mongoBase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
        _rentalHistoryCollection = mongoBase.GetCollection<RentalData>(databaseSettings.Value.RentalHistoryCollectionName); 

        // var mongoConnectionString = "mongodb://localhost:27017";
        // var mongoClient = new MongoClient(mongoConnectionString);
        // var database = mongoClient.GetDatabase("RentalHistory");
        // _rentalHistoryCollection = database.GetCollection<RentalData>("RentalHistory");

        _kafkaConfig  = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "lalalala1",
            //AutoOffsetReset = AutoOffsetReset.Latest
        };
    }
    public async Task ConsumeKafka (CancellationToken stoppingToken)
    {
        using (var consumer = new ConsumerBuilder<Ignore, string>(_kafkaConfig).Build())
        {
            
            consumer.Subscribe(_kafkaTopic);
            try 
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                
                    var message = consumer.Consume(stoppingToken);
                    var rentalData = JsonConvert.DeserializeObject<RentalData>((message.Value));
                   
                     if (rentalData.ReturnDate == null)
                     {
                        
                        _logger.LogInformation($"Save communicate from kafka to MongoDB action INSERT invoked");
                        
                        
                        await _rentalHistoryCollection.InsertOneAsync(rentalData, cancellationToken: stoppingToken);
                        _logger.LogInformation($"Save communicate from kafka to MongoDB action INSERT executed");
                         
                     }
                     else
                     {
                         _logger.LogInformation($"Save communicate from kafka to MongoDB action UPDATE invoked");
                         var filter = Builders<RentalData>.Filter.Where(r => r.Rentid == rentalData.Rentid);
                         var update = Builders<RentalData>.Update.Set(r => r.ReturnDate, rentalData.ReturnDate);
                         await _rentalHistoryCollection.FindOneAndUpdateAsync(filter, update, new FindOneAndUpdateOptions<RentalData>(), stoppingToken);
                         _logger.LogInformation($"Save communicate from kafka to MongoDB action UPDATE executed");
                     }
                      
                }   
            }
            catch (OperationCanceledException)
            {
                consumer.Close();
            }

        }
    }
    protected override  Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Task.Run(() => ConsumeKafka(stoppingToken));
        return Task.CompletedTask;
    
        
    }
}