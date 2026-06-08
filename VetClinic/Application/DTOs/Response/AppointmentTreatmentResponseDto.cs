namespace VetClinic.Application.DTOs.Response;

public record AppointmentTreatmentResponseDto(
    Guid Id,
    string Type,
    decimal Duration,
    decimal Price
);