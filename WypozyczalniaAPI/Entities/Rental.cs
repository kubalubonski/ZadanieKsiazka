using System.ComponentModel.DataAnnotations;

namespace WypozyczalniaAPI.Entities;

public class Rental 
{
    [Key]
    public int Rentid {get; set;}
    public string Name {get; set;} = null!;
    public string Surname {get; set;} = null!;
    public DateTime RentDate {get; set;}
    public int Bookid {get; set;}
    public int Customerid {get; set;}
    public Customer Customer {get; set;} 
}