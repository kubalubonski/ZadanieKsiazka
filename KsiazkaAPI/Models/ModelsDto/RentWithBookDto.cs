using System.ComponentModel.DataAnnotations;

namespace KsiazkaAPI;

public class RentWithBookDto
{
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
    [MaxLength(255)]
    public int Bookid {get; set;}
    

    public BookDto BookInfo {get; set;}
}