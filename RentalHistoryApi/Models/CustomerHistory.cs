namespace RentalHistoryAPI.Models;

using System.ComponentModel.DataAnnotations;

public class CustomerHistory
{
    
    public CustomerDto customer {get; set;}
    public List<RentalDataDto> rentalHistory {get; set;}
      


}

