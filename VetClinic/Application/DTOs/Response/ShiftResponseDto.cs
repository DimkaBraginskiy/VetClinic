namespace VetClinic.Application.DTOs.Response;

public record ShiftResponseDto(
    Guid     Id,
    DateTime StartTime,
    DateTime EndTime,
    Guid     ClinicId,
    string   ClinicName);