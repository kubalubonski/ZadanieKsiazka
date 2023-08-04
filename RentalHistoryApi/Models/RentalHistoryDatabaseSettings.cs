namespace RentalHistoryAPI;

public class RentalHistoryDatabaseSettings
{
    public string ConnectionString {get; set;} = null!;
    public string DatabaseName {get; set;} = null!;
    public string RentalHistoryCollectionName {get; set;} = null!;
}