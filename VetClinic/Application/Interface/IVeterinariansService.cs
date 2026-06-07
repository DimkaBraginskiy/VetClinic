using VetClinic.Application.DTOs;
using VetClinic.Application.DTOs.Request;

namespace VetClinic.Application.Services;

public interface IVeterinariansService
{
    public Task<Guid> CreateVeterinarianAsync(CreateVeterinarianRequestDto dto);
    public Task DeleteVeterinarianAsync(Guid id);
    public Task<List<VeterinarianDatesResponseDto>> GetAvailableVeterinariansAsync(
        DateOnly date, Guid? clinicId, string? treatmentType);
}