namespace VetClinic.Application.DTOs;

public record VeterinarianDatesResponseDto(
    Guid VeterinarianId,
    string FirstName,
    string LastName,
    string? MiddleName,
    string Type,
    string ClinicVetId,
    int AppointmentDurationMinutes,
    decimal? OfferingPrice,
    string? OfferingType,
    ICollection<DateTime> AvailableHours
);