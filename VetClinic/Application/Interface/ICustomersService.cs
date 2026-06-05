using VetClinic.Application.DTOs;

namespace VetClinic.Application.Services;

public interface ICustomersService
{
    public Task<IEnumerable<CustomerAnimalResponseDto>> GetCustomerAnimalsAsync(Guid customerId);
}