namespace VetClinic.Domain.Entities;

public class Address
{
    public string Country { get; private set; } = null!;
    public string City { get; private set; } = null!;
    public string Street { get; private set; } = null!;
    public int Building { get; private set; }
    public int? Flat { get; private set; }
    public string? PostalCode { get; private set; } = null!;
}