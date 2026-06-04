using VetClinic.Domain.Enums;

namespace VetClinic.Domain.Entities;

public class Appointment
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public AppointmentStatus Status { get; private set; } = AppointmentStatus.Scheduled;
    public AppointmentType Type { get; private set; }
    public AppointmentMode Mode { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate => StartDate.AddMinutes(GetTotalDuration());

    public decimal BasePrice { get; private set; }
    public string? PaymentDetails { get; private set; }

    // Online
    public string? MeetingLink { get; private set; }

    // Home
    public Address? HomeAddress { get; private set; }

    // Clinic
    public DateTime? ArriveTime { get; private set; }
    public Guid? CabinetId { get; private set; }
    public Cabinet? Cabinet { get; private set; }

    public Guid? TreatmentId { get; private set; }
    public Treatment? Treatment { get; private set; }
    public Guid VeterinarianId { get; private set; }
    public Veterinarian Veterinarian { get; private set; } = null!;
    public Guid CustomerId { get; private set; }
    public Customer Customer { get; private set; } = null!;

    private readonly List<Animal> _animals = new();
    private readonly List<Discount> _discounts = new();
    public IReadOnlyCollection<Animal> Animals => _animals.AsReadOnly();
    public IReadOnlyCollection<Discount> Discounts => _discounts.AsReadOnly();

    protected Appointment() { }

    private Appointment(AppointmentType type, AppointmentMode mode, DateTime startDate,
        decimal basePrice, Guid customerId, Guid veterinarianId, Treatment? treatment)
    {
        if (startDate < DateTime.Now)
            throw new ArgumentException("Start date cannot be in the past.");
        if (basePrice < 0)
            throw new ArgumentException("Base price cannot be negative.");
        if (customerId == Guid.Empty || veterinarianId == Guid.Empty)
            throw new ArgumentException("Customer and Veterinarian are required.");
        if (type == AppointmentType.Treatment && treatment == null)
            throw new InvalidOperationException("Treatment appointment must have a Treatment.");
        if (type == AppointmentType.Consultation && treatment != null)
            throw new InvalidOperationException("Consultation appointment cannot have a Treatment.");

        Type = type;
        Mode = mode;
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
    
    public static Appointment CreateOnline(AppointmentType type, DateTime startDate,
        decimal basePrice, Guid customerId, Guid veterinarianId, string? meetingLink = null)
    {
        if (type == AppointmentType.Treatment)
            throw new ArgumentException("Online appointments can only be Consultations.");

        var appt = new Appointment(type, AppointmentMode.Online, startDate,
            basePrice, customerId, veterinarianId, treatment: null);
        appt.MeetingLink = meetingLink ?? $"https://meet.vetclinic.com/{Guid.NewGuid()}";
        return appt;
    }

    public static Appointment CreateHome(AppointmentType type, DateTime startDate,
        decimal basePrice, Guid customerId, Guid veterinarianId,
        Address address, Treatment? treatment = null)
    {
        ArgumentNullException.ThrowIfNull(address);

        var appt = new Appointment(type, AppointmentMode.Home, startDate,
            basePrice, customerId, veterinarianId, treatment);
        appt.HomeAddress = address;
        return appt;
    }

    public static Appointment CreateClinic(AppointmentType type, DateTime startDate,
        decimal basePrice, Guid customerId, Guid veterinarianId,
        DateTime arriveTime, Cabinet cabinet, Treatment? treatment = null)
    {
        ArgumentNullException.ThrowIfNull(cabinet);
        if (arriveTime >= startDate)
            throw new ArgumentException("Arrival time must be before start time.");

        var appt = new Appointment(type, AppointmentMode.Clinic, startDate,
            basePrice, customerId, veterinarianId, treatment);
        appt.ArriveTime = arriveTime;
        appt.CabinetId = cabinet.Id;
        appt.Cabinet = cabinet;
        return appt;
    }

    public void ChangeToOnline()
    {
        if (Mode == AppointmentMode.Online)
            throw new InvalidOperationException("Already in Online mode.");
        if (Status != AppointmentStatus.Scheduled)
            throw new InvalidOperationException("Mode can only be changed while Scheduled.");
        if (Type == AppointmentType.Treatment)
            throw new InvalidOperationException("Treatment appointments cannot be switched to Online.");

        HomeAddress = null;
        ArriveTime = null;
        CabinetId = null;
        Cabinet = null;

        Mode = AppointmentMode.Online;
        MeetingLink = $"https://meet.vetclinic.com/{Guid.NewGuid()}";
    }

    public void ChangeToHome(Address address)
    {
        if (Mode == AppointmentMode.Home)
            throw new InvalidOperationException("Already in Home mode.");
        if (Status != AppointmentStatus.Scheduled)
            throw new InvalidOperationException("Mode can only be changed while Scheduled.");
        ArgumentNullException.ThrowIfNull(address);

        MeetingLink = null;
        ArriveTime = null;
        CabinetId = null;
        Cabinet = null;

        Mode = AppointmentMode.Home;
        HomeAddress = address;
    }

    public void ChangeToClinic(DateTime arriveTime, Cabinet cabinet)
    {
        if (Mode == AppointmentMode.Clinic)
            throw new InvalidOperationException("Already in Clinic mode.");
        if (Status != AppointmentStatus.Scheduled)
            throw new InvalidOperationException("Mode can only be changed while Scheduled.");
        ArgumentNullException.ThrowIfNull(cabinet);
        if (arriveTime >= StartDate)
            throw new ArgumentException("Arrival time must be before start time.");

        MeetingLink = null;
        HomeAddress = null;

        Mode = AppointmentMode.Clinic;
        ArriveTime = arriveTime;
        CabinetId = cabinet.Id;
        Cabinet = cabinet;
    }

    public void TransitionTo(AppointmentStatus newStatus)
    {
        if (!IsValidTransition(Status, newStatus))
            throw new InvalidOperationException($"Cannot transition from {Status} to {newStatus}.");
        Status = newStatus;
    }

    private bool IsValidTransition(AppointmentStatus current, AppointmentStatus next) =>
        (current, next) switch
        {
            (AppointmentStatus.Scheduled,  AppointmentStatus.InProgress) => true,
            (AppointmentStatus.Scheduled,  AppointmentStatus.Cancelled)  => true,
            (AppointmentStatus.InProgress, AppointmentStatus.Completed)  => true,
            _ => false
        };

    // --- Collections ---

    public void AddAnimal(Animal animal)
    {
        ArgumentNullException.ThrowIfNull(animal);
        if (_animals.Any(a => a.Id == animal.Id))
            throw new InvalidOperationException("Animal already added.");
        _animals.Add(animal);
    }

    public void RemoveAnimal(Guid animalId)
    {
        var animal = _animals.FirstOrDefault(a => a.Id == animalId)
            ?? throw new ArgumentException("Animal not found.");
        _animals.Remove(animal);
    }

    public void AddDiscount(Discount discount)
    {
        ArgumentNullException.ThrowIfNull(discount);
        if (_discounts.Any(d => d.Id == discount.Id))
            throw new InvalidOperationException("Discount already applied.");
        _discounts.Add(discount);
    }

    public decimal GetTotalPrice()
    {
        var discount = _discounts.Sum(d => d.Percentage) / 100m;
        var base_ = Treatment == null ? BasePrice : BasePrice + Treatment.Price;
        return base_ * (1 - discount);
    }

    public string GetTitle()
    {
        var animalPart = Animals.Count > 1
            ? "multiple animals"
            : Animals.FirstOrDefault()?.Name ?? "unknown";

        return Treatment == null
            ? $"Consultation for {animalPart}"
            : $"{Treatment.Type} for {animalPart}";
    }

    public int GetTotalDuration() =>
        Treatment != null ? (int)Treatment.Duration : 30;
}