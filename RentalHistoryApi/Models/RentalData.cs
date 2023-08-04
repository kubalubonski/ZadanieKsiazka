using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RentalHistoryAPI.Models;


public class RentalData
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id {get; set;}
    [BsonElement("rentid")]
    public int Rentid {get; set;}
    //[BsonElement("action")]
    // public string Action { get; set; }
     [BsonElement("customerid")]
    public int Customerid {get; set;}
    [BsonElement("name")]
    public string Name { get; set; }
    //[BsonElement("surname")]
    // public string Surname { get; set; }
    [BsonElement("rentdate")]
    public DateTime RentDate { get; set; }
    [BsonElement("returndate")]
    public DateTime? ReturnDate { get; set; }
    [BsonElement("bookid")]
    public int Bookid { get; set; }
    // [BsonElement("bookinfo")]
    // public BookInfoData BookInfo { get; set; }
    



}