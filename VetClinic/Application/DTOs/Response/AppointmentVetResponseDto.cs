namespace VetClinic.Application.DTOs;

public record AppointmentVetResponseDto(
    Guid Id,
    string FullName,
    string Type,
    string ClinicVetId
);