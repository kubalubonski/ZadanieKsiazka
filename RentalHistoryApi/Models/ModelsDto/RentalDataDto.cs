namespace RentalHistoryAPI.Models;


public class RentalDataDto
{
    public DateTime RentDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public int Bookid { get; set; }
    public string Title {get; set;}

}