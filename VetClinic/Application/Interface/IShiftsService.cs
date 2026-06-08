using VetClinic.Application.DTOs.Request;
using VetClinic.Application.DTOs.Response;

namespace VetClinic.Application.Interface;


public interface IShiftsService
{
    Task<Guid> CreateShiftAsync(CreateShiftRequestDto dto);
    Task DeleteShiftAsync(Guid id);
    Task<List<ShiftResponseDto>> GetVetShiftsAsync(Guid veterinarianId);
}