namespace VetClinic.Domain.Entities;

public class Discount
{
    public decimal Percentage { get; private set; }
    public string PromoCode { get; private set; } = null!;
}