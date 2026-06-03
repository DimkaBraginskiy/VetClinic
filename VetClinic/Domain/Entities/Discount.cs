namespace VetClinic.Domain.Entities;

public class Discount
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    
    public decimal Percentage { get; private set; }
    public string PromoCode { get; private set; } = null!;
}