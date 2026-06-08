namespace VetClinic.Application.DTOs.Response;

public record AppointmentVetResponseDto(
    Guid Id,
    string FullName,
    string Type,
    string ClinicVetId
);