namespace VetClinic.Application.DTOs;

public record AddressResponseDto(
    string Country,
    string City,
    string Street,
    int Building,
    int? Flat,
    string? PostalCode
);