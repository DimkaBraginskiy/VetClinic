namespace VetClinic.Application.DTOs;

public record AppointmentTreatmentResponseDto(
    Guid Id,
    string Type,
    decimal Duration,
    decimal Price
);