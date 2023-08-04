using System.ComponentModel.DataAnnotations;

namespace WypozyczalniaAPI.Entities;


public class Customer 
{

     [Key]
    public int Customerid {get; set;}
    public string Name {get; set;}
    public string Surname {get; set;}
    public ICollection<Rental> Rentals {get;} = new List<Rental>();

}