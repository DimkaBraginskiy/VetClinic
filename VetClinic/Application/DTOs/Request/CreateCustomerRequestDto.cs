namespace VetClinic.Application.DTOs.Request;

public class CreateCustomerRequestDto
{
    public string  FirstName     { get; set; } = null!;
    public string  LastName      { get; set; } = null!;
    public string  Email         { get; set; } = null!;
    public string? MiddleName    { get; set; }
    public decimal LoyaltyPoints { get; set; } = 0;
}