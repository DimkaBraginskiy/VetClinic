using VetClinic.Application.DTOs.Request;
using VetClinic.Application.DTOs.Response;

namespace VetClinic.Application.Interface;

public interface IClinicService
{
    public Task<ICollection<ClinicResponseDto>> GetAllClinicsAsync();
    public Task<Guid> CreateClinicAsync(CreateClinicRequestDto dto);
    public Task<Guid> AddCabinetAsync(Guid clinicId, AddCabinetRequestDto dto);
}