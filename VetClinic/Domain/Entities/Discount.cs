namespace VetClinic.Domain.Entities;

public class Discount
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public decimal Percentage { get; private set; }
    public string PromoCode { get; private set; } = null!;

    protected Discount() { }

    public Discount(decimal percentage, string promoCode)
    {
        if (percentage <= 0 || percentage > 100)
            throw new ArgumentException("Percentage must be between 0 and 100 percents");
        if (string.IsNullOrWhiteSpace(promoCode))
            throw new ArgumentException("Promo code is required.");

        Percentage = percentage;
        PromoCode = promoCode.Trim().ToUpper();
    }
}