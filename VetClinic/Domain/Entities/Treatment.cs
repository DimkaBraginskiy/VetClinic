namespace VetClinic.Domain.Entities;

public class Treatment
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    
    public TreatmentType Type { get; private set; }
    public TreatmentStatus Status { get; private set; }
    public decimal Duration { get; private set; }   // e.g. minutes
    public decimal Price { get; private set; }
    
    public Veterinarian Veterinarian { get; private set; } = null!;
    public Appointment? Appointment { get; private set; }

    private Treatment() { }
}