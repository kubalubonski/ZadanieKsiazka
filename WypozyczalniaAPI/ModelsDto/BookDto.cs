using System.ComponentModel.DataAnnotations;
using WypozyczalniaAPI.Entities;

namespace WypozyczalniaAPI;

public class BookDto 
{
    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = null!;
    [Required]
    [MaxLength(255)]
    public string Author { get; set; } = null!;
    [Required]
    [DataType(DataType.Date)]
    public DateTime ReleaseDate { get; set; }
}