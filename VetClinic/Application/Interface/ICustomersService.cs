using VetClinic.Application.DTOs.Request;
using VetClinic.Application.DTOs.Response;

namespace VetClinic.Application.Interface;

public interface ICustomersService
{
    public Task<Guid>  CreateCustomerAsync(CreateCustomerRequestDto dto);
    public Task<Guid?> GetCustomerIdByEmailAsync(string email);
    public Task<IEnumerable<CustomerAnimalResponseDto>> GetCustomerAnimalsAsync(Guid customerId);
    public Task<Guid>  CreateCustomerAnimalAsync(CreatAnimalRequestDto dto, Guid customerId);
    public Task        DeleteCustomerAnimalAsync(Guid customerId, Guid animalId);
    public Task<int>     GetAppointmentCountAsync(Guid customerId);
    public Task<decimal> GetLoyaltyPointsAsync(Guid customerId);
    public Task<CustomerAnimalResponseDto> GetAnimalByIdAsync(Guid customerId, Guid animalId);
}