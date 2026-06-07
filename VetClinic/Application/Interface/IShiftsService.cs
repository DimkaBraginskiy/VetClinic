using VetClinic.Application.DTOs.Request;

namespace VetClinic.Application.Services;

public interface IShiftsService
{
    Task<Guid> CreateShiftAsync(CreateShiftRequestDto dto);
    Task DeleteShiftAsync(Guid id);
}