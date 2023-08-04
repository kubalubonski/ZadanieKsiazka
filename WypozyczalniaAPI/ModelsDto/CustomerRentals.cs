namespace WypozyczalniaAPI.ModelsDto;
using WypozyczalniaAPI.Entities;
using System;
using System.ComponentModel.DataAnnotations;

public class CustomerRentals
{
    [Required]
    public string Name {get; set;}
    [Required]
    public string Surname {get; set;}
    public required List<RentalDto> Rentals {get; set;}

  
    
}