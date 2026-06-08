namespace VetClinic.Application.DTOs.Response;

public record LoginResponseDto(
    bool    Success,
    Guid?   CustomerId     = null,
    Guid?   VeterinarianId = null,
    string? Message        = null);