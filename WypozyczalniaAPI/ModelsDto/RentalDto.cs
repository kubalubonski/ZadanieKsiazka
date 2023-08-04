using System.ComponentModel.DataAnnotations;
using WypozyczalniaAPI.Entities;

namespace WypozyczalniaAPI;

public class RentalDto 
{
    
    public int Customerid {get; set;}
    [Required]
    [MaxLength(255)]
    public string Name {get; set;} = null!;

    [Required]
    [MaxLength(255)]
    public string Surname {get; set;} = null!;

    [Required]
    [DataType(DataType.Date)]
    public DateTime RentDate {get; set;}
    
    [Required]
    public int Bookid {get; set;}
    
    


}