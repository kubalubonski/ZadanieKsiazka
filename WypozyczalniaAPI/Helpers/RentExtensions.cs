using WypozyczalniaAPI.Entities;

namespace WypozyczalniaAPI;

public static class RentExtensions
{
    public static RentalDto ToDto(this Rental rental)
    {
        if(rental != null)
        {
            return new RentalDto
            {
                Name = rental.Name,
                Surname = rental.Surname,
                RentDate = rental.RentDate,
                Bookid = rental.Bookid,
                Customerid = rental.Customerid
            };
        }
        return null;
    }

    public static Rental ToRental(this RentalDto dto)
    {
        if(dto != null)
        {
            return new Rental
            {
                Name = dto.Name,
                Surname = dto.Surname,
                RentDate = dto.RentDate,
                Bookid = dto.Bookid,
                Customerid = dto.Customerid
            };
        }
        return null;

    }
}