namespace VetClinic.Application.DTOs.Response;

public record ClinicCabinetResponseDto(
    Guid Id,
    int  Floor,
    int  Number
);