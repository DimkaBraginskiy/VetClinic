namespace VetClinic.Application.DTOs.Response;

public record AppointmentAnimalResponseDto(
    Guid Id,
    string Name,
    string? Species,
    string? Breed
);