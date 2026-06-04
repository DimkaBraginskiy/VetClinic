namespace VetClinic.Domain.Entities;

public class Treatment
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public TreatmentType Type { get; private set; }
    public TreatmentStatus Status { get; private set; } = TreatmentStatus.Scheduled;
    public decimal Duration { get; private set; }
    public decimal Price { get; private set; }

    public Guid VeterinarianId { get; private set; }
    public Veterinarian Veterinarian { get; private set; } = null!;

    public Guid? AppointmentId { get; private set; }
    public Appointment? Appointment { get; private set; }

    protected Treatment() { }

    public Treatment(TreatmentType type, decimal duration, decimal price, Veterinarian veterinarian)
    {
        if (duration <= 0) throw new ArgumentException("Duration must be positive.");
        if (price < 0)     throw new ArgumentException("Price cannot be negative.");
        ArgumentNullException.ThrowIfNull(veterinarian);

        Type = type;
        Duration = duration;
        Price = price;
        VeterinarianId = veterinarian.Id;
        Veterinarian = veterinarian;
    }
}