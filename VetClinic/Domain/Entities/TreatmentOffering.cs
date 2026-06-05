using VetClinic.Domain.Enums;

namespace VetClinic.Domain.Entities;

public class TreatmentOffering
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public TreatmentType Type { get; private set; }
    public decimal Price { get; private set; }
    public decimal BaseDuration { get; private set; }

    public Guid VeterinarianId { get; private set; }
    public Veterinarian Veterinarian { get; private set; } = null!;

    protected TreatmentOffering() { }

    internal TreatmentOffering(TreatmentType type, decimal price, decimal baseDuration, Veterinarian vet)
    {
        if (price < 0)         throw new ArgumentException("Price cannot be negative.");
        if (baseDuration <= 0) throw new ArgumentException("Duration must be positive.");
        if (type is TreatmentType.Surgery or TreatmentType.Chiropractic
            && vet.Type != VeterinarianType.Surgeon)
            throw new InvalidOperationException("Only a Surgeon can offer Surgery or Chiropractic.");

        Type = type;
        Price = price;
        BaseDuration = baseDuration;
        VeterinarianId = vet.Id;
        Veterinarian = vet;
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0) throw new ArgumentException("Price cannot be negative.");
        Price = newPrice;
    }

    public void UpdateDuration(decimal newDuration)
    {
        if (newDuration <= 0) throw new ArgumentException("Duration must be positive.");
        BaseDuration = newDuration;
    }
}