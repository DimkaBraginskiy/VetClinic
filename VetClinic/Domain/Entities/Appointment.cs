using System.Collections.ObjectModel;
using VetClinic.Domain.Enums;

namespace VetClinic.Domain.Entities;

public abstract class Appointment
{
    public Guid Id { get; private set; }
    public AppointmentStatus Status { get; private set; } = AppointmentStatus.Scheduled;
    public AppointmentType Type { get; private set; }
    public DateTime StartDate { get; private set; }
    public int DurationMinutes { get; private set; }
    public DateTime EndDate => StartDate.AddMinutes(DurationMinutes);
    
    public decimal BasePrice { get; private set; }
    public string? PaymentDetails { get; private set; }

    public Guid? TreatmentId { get; private set; }
    public Treatment? Treatment { get; private set; }
    public Guid VeterinarianId { get; private set; }
    public Veterinarian Veterinarian { get; private set; } = null!;
    
    public Guid CustomerId { get; private set; }
    public Customer Customer { get; private set; } = null!;

    private readonly List<Animal> _animals = new();
    private readonly List<Discount> _discounts = new();
    public IReadOnlyCollection<Animal> Animals => _animals.AsReadOnly();
    public ICollection<Discount> Discounts => _discounts.AsReadOnly();

    public Appointment() { }

    protected Appointment(
        AppointmentType type,
        DateTime startDate,
        int durationMinutes,
        decimal basePrice,
        Guid customerId,
        Guid veterinarianId,
        Treatment? treatment = null)
    {
        Validate(type, startDate, durationMinutes, basePrice, customerId, veterinarianId, treatment);

        Type = type;
        StartDate = startDate;
        DurationMinutes = durationMinutes;
        BasePrice = basePrice;
        CustomerId = customerId;
        VeterinarianId = veterinarianId;

        if (treatment != null)
        {
            TreatmentId = treatment.Id;
            Treatment = treatment;
        }
    }
    
    private void Validate(AppointmentType type, DateTime startDate, int durationMinutes, 
        decimal basePrice, Guid customerId, Guid vetId, Treatment? treatment)
    {
        if (startDate < DateTime.Now)
            throw new ArgumentException("Appointment start date cannot be in the past.");

        if (durationMinutes <= 0)
            throw new ArgumentException("Duration must be positive.");

        if (basePrice < 0)
            throw new ArgumentException("Base price cannot be negative.");

        if (customerId == Guid.Empty || vetId == Guid.Empty)
            throw new ArgumentException("Customer and Veterinarian are required.");
        
        if (type == AppointmentType.Treatment && treatment == null)
            throw new InvalidOperationException("Treatment Appointment must have a Treatment.");

        if (type == AppointmentType.Consultation && treatment != null)
            throw new InvalidOperationException("Consultation Appointment cannot have a Treatment.");
    }
    
    public void AddAnimal(Animal animal)
    {
        if (animal == null)
            throw new ArgumentNullException(nameof(animal));

        if (_animals.Any(a => a.Id == animal.Id))
            throw new InvalidOperationException("Animal already added to this appointment.");

        _animals.Add(animal);
    }

    public void RemoveAnimal(Guid animalId)
    {
        var animal = _animals.FirstOrDefault(a => a.Id == animalId);
        if (animal == null)
            throw new ArgumentException("Animal not found in this appointment.");

        _animals.Remove(animal);
    }

    public void AddDiscount(Discount discount)
    {
        if (discount == null)
            throw new ArgumentNullException(nameof(discount));

        if (_discounts.Any(d => d.Id == discount.Id))
            throw new InvalidOperationException("Discount already applied.");

        _discounts.Add(discount);
    }
    
    public void ChangeStatus(AppointmentStatus newStatus)
    {
        
    }
}