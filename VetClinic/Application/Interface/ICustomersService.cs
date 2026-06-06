using VetClinic.Application.DTOs;
using VetClinic.Application.DTOs.Request;

namespace VetClinic.Application.Services;

public interface ICustomersService
{
    public Task<Guid>  CreateCustomerAsync(CreateCustomerRequestDto dto);
    public Task<Guid?> GetCustomerIdByEmailAsync(string email);
    public Task<IEnumerable<CustomerAnimalResponseDto>> GetCustomerAnimalsAsync(Guid customerId);
    public Task<Guid>  CreateCustomerAnimalAsync(CreatAnimalRequestDto dto, Guid customerId);
    public Task        DeleteCustomerAnimalAsync(Guid customerId, Guid animalId);
}