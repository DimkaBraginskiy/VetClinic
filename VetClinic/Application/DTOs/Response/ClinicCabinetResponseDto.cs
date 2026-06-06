namespace VetClinic.Application.DTOs;

public record ClinicCabinetResponseDto(
    Guid Id,
    int  Floor,
    int  Number
);