using VetClinic.Application.DTOs;
using VetClinic.Application.DTOs.Request;

namespace VetClinic.Application.Services;

public interface IClinicService
{
    public Task<ICollection<ClinicResponseDto>> GetAllClinicsAsync();
    public Task<Guid> CreateClinicAsync(CreateClinicRequestDto dto);
    public Task<Guid> AddCabinetAsync(Guid clinicId, AddCabinetRequestDto dto);
}