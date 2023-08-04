using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KsiazkaAPI.Models;

public partial class Book
{
    [Key]
    public int Bookid { get; set; }

    public string Title { get; set; } = null!;

    public string Author { get; set; } = null!;

    public DateOnly ReleaseDate { get; set; }
    
}
