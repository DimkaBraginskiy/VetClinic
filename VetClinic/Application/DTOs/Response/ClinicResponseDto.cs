namespace VetClinic.Application.DTOs.Response;

public record ClinicResponseDto(
    Guid   Id,
    string Name,
    string Country,
    string City,
    string Street,
    int    Building,
    string PostalCode,
    ICollection<ClinicCabinetResponseDto> Cabinets
);