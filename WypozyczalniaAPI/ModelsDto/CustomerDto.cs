namespace WypozyczalniaAPI.ModelsDto;
using System.ComponentModel.DataAnnotations;

public class CustomerDto
{
    [Required]
    public string Name {get; set;}
    [Required]
    public string Surname {get; set;}
      


}