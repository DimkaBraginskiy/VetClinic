namespace VetClinic.Application.DTOs.Response;

public record AddressResponseDto(
    string Country,
    string City,
    string Street,
    int Building,
    int? Flat,
    string? PostalCode
);