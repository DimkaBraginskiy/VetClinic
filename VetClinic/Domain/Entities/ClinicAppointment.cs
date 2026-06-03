namespace VetClinic.Domain.Entities;

public class ClinicAppointment : Appointment
{
    public Guid Id { get; private set; }
    
    public DateTime ArriveTime { get; private set; }
    
    private readonly List<Clinic.Cabinet> _cabinets = new();
    public IReadOnlyCollection<Clinic.Cabinet> Cabinets => _cabinets.AsReadOnly();
    
    
}