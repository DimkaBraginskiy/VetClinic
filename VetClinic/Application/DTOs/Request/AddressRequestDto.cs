namespace VetClinic.Application.DTOs.Request;

public class AddressRequestDto
{
    public string Country { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Street { get; set; } = null!;
    public int Building { get; set; }
    public int? Flat { get; set; }
    public string? PostalCode { get; set; }
}