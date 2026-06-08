using VetClinic.Application.DTOs.Request;
using VetClinic.Application.DTOs.Response;

namespace VetClinic.Application.Interface;

public interface IVeterinariansService
{
    public Task<Guid> CreateVeterinarianAsync(CreateVeterinarianRequestDto dto);
    public Task DeleteVeterinarianAsync(Guid id);
    public Task<List<VeterinarianDatesResponseDto>> GetAvailableVeterinariansAsync(
        DateOnly date, Guid? clinicId, string? treatmentType);
    public Task<Guid?> GetVetIdByEmailAsync(string email);
    public Task<VetProfileDto> GetVetProfileAsync(Guid id);
}