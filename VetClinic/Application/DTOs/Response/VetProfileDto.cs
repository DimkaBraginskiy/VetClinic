namespace VetClinic.Application.DTOs.Response;

public record VetProfileDto(
    Guid   Id,
    string FullName,
    string Type,
    string ClinicVetId,
    Guid   ClinicId,
    string ClinicName);