namespace VetClinic.Application.DTOs;

public record AppointmentAnimalResponseDto(
    Guid Id,
    string Name,
    string? Species,
    string? Breed
);