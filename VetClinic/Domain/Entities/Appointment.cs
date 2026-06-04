using System.Collections.ObjectModel;
using VetClinic.Domain.Enums;

namespace VetClinic.Domain.Entities;

public abstract class Appointment
{
    public Guid Id { get; private set; }
    public AppointmentStatus Status { get; private set; } = AppointmentStatus.Scheduled;
    public AppointmentType Type { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate => StartDate.AddMinutes(GetTotalDuration());
    
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

    protected Appointment() { }

    protected Appointment(
        AppointmentType type,
        DateTime startDate,
        decimal basePrice,
        Guid customerId,
        Guid veterinarianId,
        Treatment? treatment = null)
    {
        Validate(type, startDate, basePrice, customerId, veterinarianId, treatment);

        Type = type;
        StartDate = startDate;
        BasePrice = basePrice;
        CustomerId = customerId;
        VeterinarianId = veterinarianId;

        if (treatment != null)
        {
            TreatmentId = treatment.Id;
            Treatment = treatment;
        }
    }

    private void Validate(AppointmentType type, DateTime startDate, 
        decimal basePrice, Guid customerId, Guid vetId, Treatment? treatment)
    {
        if (startDate < DateTime.Now)
            throw new ArgumentException("Appointment start date cannot be in the past.");

        if (basePrice < 0)
            throw new ArgumentException("Base price cannot be negative.");

        if (customerId == Guid.Empty || vetId == Guid.Empty)
            throw new ArgumentException("Customer and Veterinarian are required.");
        
        if (type == AppointmentType.Treatment && treatment == null)
            throw new InvalidOperationException("Treatment Appointment must have a Treatment.");

        if (type == AppointmentType.Consultation && treatment != null)
            throw new InvalidOperationException("Consultation Appointment cannot have a Treatment.");
    }
    
    public OnlineAppointment ChangeToOnline()
    {
        if (this is OnlineAppointment)
            throw new InvalidOperationException("Appointment is already in Online mode.");
        if (Status != AppointmentStatus.Scheduled)
            throw new InvalidOperationException("Mode can only be changed while Scheduled.");
        if (Type == AppointmentType.Treatment)
            throw new InvalidOperationException("Treatment appointments cannot be switched to Online.");

        var newAppt = new OnlineAppointment(Type, StartDate, BasePrice, CustomerId, VeterinarianId);
        CopyCommonStateTo(newAppt);
        return newAppt;
    }

    public HomeAppointment ChangeToHome(Address homeAddress)
    {
        if (this is HomeAppointment)
            throw new InvalidOperationException("Appointment is already in Home mode.");
        if (Status != AppointmentStatus.Scheduled)
            throw new InvalidOperationException("Mode can only be changed while Scheduled.");
        ArgumentNullException.ThrowIfNull(homeAddress);

        var newAppt = new HomeAppointment(Type, StartDate, BasePrice, CustomerId, VeterinarianId, homeAddress, Treatment);
        CopyCommonStateTo(newAppt);
        return newAppt;
    }

    public ClinicAppointment ChangeToClinic(DateTime arriveTime, Cabinet cabinet)
    {
        if (this is ClinicAppointment)
            throw new InvalidOperationException("Appointment is already in Clinic mode.");
        if (Status != AppointmentStatus.Scheduled)
            throw new InvalidOperationException("Mode can only be changed while Scheduled.");
        ArgumentNullException.ThrowIfNull(cabinet);

        var newAppt = new ClinicAppointment(Type, StartDate, BasePrice, CustomerId, VeterinarianId, arriveTime, cabinet, Treatment);
        CopyCommonStateTo(newAppt);
        return newAppt;
    }


    private void CopyCommonStateTo(Appointment target)
    {
        foreach (var animal in _animals)
            target.AddAnimal(animal);
        foreach (var discount in _discounts)
            target.AddDiscount(discount);
    }

    public void TransitionTo(AppointmentStatus newStatus)
    {
        if(!IsValidTransition(Status, newStatus))
            throw new InvalidOperationException($"Cannot transition from {Status} to {newStatus}.");

        Status = newStatus;
    }

    private bool IsValidTransition(AppointmentStatus current, AppointmentStatus toStatus)
    {
        return (current, toStatus) switch
        {
            (AppointmentStatus.Scheduled, AppointmentStatus.InProgress) => true,
            (AppointmentStatus.Scheduled, AppointmentStatus.Cancelled) => true,
            (AppointmentStatus.InProgress, AppointmentStatus.Completed) => true,
            _ => false
        };
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
    
    public decimal GetTotalPrice()
    {
        return BasePrice + Treatment.Price;
    }
    public string GetTitle()
    {
        return Animals.Count > 1 ? $"{Treatment.Type} for multiple animals" : $"{Treatment.Type} for {Animals.FirstOrDefault()?.Name}";
    }

    public int GetTotalDuration()
    {
        return Type == AppointmentType.Treatment ? (int)Treatment.Duration : 30;
    }
}