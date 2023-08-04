using System.ComponentModel.DataAnnotations;
using WypozyczalniaAPI.Entities;

namespace WypozyczalniaAPI;

public class RentWithBookDto
{
    [Required]
    [MaxLength(255)]
    public string Name {get; set;} = null!;
    
    [Required]
    [MaxLength(255)]
    public string Surname {get; set;} = null!;
    
    [Required]
    [MaxLength(255)]
    public DateTime RentDate {get; set;}
    [Required]
    public int Customerid {get; set;}

    [Required]
    public int Bookid {get; set;}
    

    public BookDto BookInfo {get; set;}
    
    /*
    public string Title { get; set; } = null!;
    [MaxLength(255)]
    public string Author { get; set; } = null!;

    [DataType(DataType.Date)]
    public DateTime ReleaseDate { get; set; }
    */

}