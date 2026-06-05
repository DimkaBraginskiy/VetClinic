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

        if (!veterinarian.CanPerform(type))  // ← validates against capability list
            throw new InvalidOperationException($"Veterinarian cannot perform {type}.");

        Type = type;
        Duration = duration;
        Price = price;
        VeterinarianId = veterinarian.Id;
        Veterinarian = veterinarian;

        veterinarian.AddTreatment(this);
    }
    
    internal void AssignToVeterinarian(Veterinarian vet)
    {
        VeterinarianId = vet.Id;
        Veterinarian = vet;
    }
    
    public void TransitionTo(TreatmentStatus next)
    {
        bool valid = (Status, next) switch
        {
            (TreatmentStatus.Scheduled,  TreatmentStatus.InProgress) => true,
            (TreatmentStatus.InProgress, TreatmentStatus.Completed)  => true,
            _ => false
        };
        if (!valid)
            throw new InvalidOperationException($"Cannot transition from {Status} to {next}.");
        Status = next;
    }
    
    internal void RemoveCancelled()
    {
        AppointmentId = null;
        Appointment = null;
    }
}