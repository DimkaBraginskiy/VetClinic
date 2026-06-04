namespace VetClinic.Domain.Entities;

public class Address
{
    public string Country { get; private set; } = null!;
    public string City { get; private set; } = null!;
    public string Street { get; private set; } = null!;
    public int Building { get; private set; }
    public int? Flat { get; private set; }
    public string? PostalCode { get; private set; }

    protected Address() { }

    public Address(string country, string city, string street, int building,
        int? flat = null, string? postalCode = null)
    {
        if (string.IsNullOrWhiteSpace(country)) throw new ArgumentException("Country required.");
        if (string.IsNullOrWhiteSpace(city))    throw new ArgumentException("City required.");
        if (string.IsNullOrWhiteSpace(street))  throw new ArgumentException("Street required.");
        if (building <= 0)                      throw new ArgumentException("Building must be positive.");

        Country = country;
        City = city;
        Street = street;
        Building = building;
        Flat = flat;
        PostalCode = postalCode;
    }
}