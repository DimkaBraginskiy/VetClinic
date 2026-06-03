using VetClinic.Domain.Enums;

namespace VetClinic.Domain.Entities;

public abstract class Appointment
{
    public Guid Id { get; private set; }
    
    public AppointmentStatus Status { get; private set; }
    public AppointmentType Type { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public decimal BasePrice { get; private set; }
    public string? PaymentDetails { get; private set; }

    public Guid? TreatmentId { get; private set; }
    public Treatment? Treatment { get; private set; }
    
    public Guid VeterinarianId { get; private set; }
    public Veterinarian Veterinarian { get; private set; } = null!;
    
    
    public ICollection<Animal> Animals { get; private set; } = new List<Animal>();
    public ICollection<Customer> Customers { get; private set; } = new List<Customer>();
    public ICollection<Discount> Discounts { get; private set; } = new List<Discount>();

    public Appointment() { }

    public Appointment(DateTime startDate, DateTime endDate, Guid veterinarianId)
    {
        StartDate = startDate;
        EndDate = endDate;
        VeterinarianId = veterinarianId;
    }
}