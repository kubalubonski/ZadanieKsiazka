using RentalHistoryAPI.Models;

namespace RentalHistoryAPI.Helpers;

public static class RentExtensions
{
    public static RentalDataDto ToDto(this RentalData rentalData)
    {
        if(rentalData != null)
        {
            return new RentalDataDto
            {
                Bookid = rentalData.Bookid,
                RentDate = rentalData.RentDate,
                ReturnDate = rentalData.ReturnDate
            };
        }
        return null;
    }

    public static RentalData ToRentalData(this RentalDataDto dto)
    {
        if(dto != null)
        {
            return new RentalData
            {                
                Bookid = dto.Bookid,
                RentDate = dto.RentDate,
                ReturnDate = dto.ReturnDate
            };
        }
        return null;

    }
}