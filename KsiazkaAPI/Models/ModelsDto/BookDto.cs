using System.ComponentModel.DataAnnotations;
using KsiazkaAPI.Filters;
namespace KsiazkaAPI;

public class BookDto
{
    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = null!;
    [Required]
    [MaxLength(255)]
    public string Author { get; set; } = null!;

    [DataType(DataType.Date)]
    [ReleaseDate(ErrorMessage = "Release date cannot be later than current date")]
    public DateTime ReleaseDate { get; set; }

}

