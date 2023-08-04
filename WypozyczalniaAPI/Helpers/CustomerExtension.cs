using WypozyczalniaAPI.Entities;
using WypozyczalniaAPI.ModelsDto;
namespace WypozyczalniaAPI;

public static class CustomerExtensions
{
    public static CustomerDto ToCustomerDto(this Customer customer)
    {
        if(customer != null)
        {
            return new CustomerDto
            {
                Name = customer.Name,
                Surname = customer.Surname,
            };
        }
        return null;
    }

    public static Customer ToCustomer(this CustomerDto dto)
    {
        if(dto != null)
        {
            return new Customer
            {
                Name = dto.Name,
                Surname = dto.Surname,
            };
        }
        return null;

    }
}