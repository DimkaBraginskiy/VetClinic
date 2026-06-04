using VetClinic.Domain.Enums;

namespace VetClinic.Domain.Entities;

public class Cabinet
{
    public Guid Id { get; private set; } = Guid.NewGuid();
        
    public int Floor { get; private set; }
    public int Number { get; private set; }
        
    public Clinic Clinic { get; private set; }
    public Guid ClinicId { get; private set; }
        
    private readonly List<Appointment> _appointments = new();
    public IReadOnlyCollection<Appointment> Appointments =>
        _appointments.AsReadOnly();

    protected Cabinet() { }

    internal Cabinet(int floor, int number, Clinic clinic)
    {
        if (floor < 0)   throw new ArgumentException("Floor cannot be negative.");
        if (number <= 0) throw new ArgumentException("Number must be positive.");
        ArgumentNullException.ThrowIfNull(clinic);

        Floor = floor;
        Number = number;
        Clinic = clinic;
        ClinicId = clinic.Id;
    }
        
    private void Validate(int floor, int number, Clinic clinic)
    {
        if (floor < 0)
        {
            throw new ArgumentException("Floor can not be negative");
        }

        if (number <= 0)
        {
            throw new ArgumentException("Number must be greater than 0");
        }

        if (clinic == null)
        {
            throw new ArgumentException("Clinic can not be null");
        }
    }
        
    public bool IsAvailable(DateTime requestedStart, DateTime requestedEnd)
    {
        return !_appointments.Any(a =>
            a.Status != AppointmentStatus.Cancelled &&
            requestedStart < a.EndDate &&
            requestedEnd > a.StartDate);
    }

    public void AddAppointment(Appointment appointment)       
    {
        if(appointment.Mode != AppointmentMode.Clinic)
            throw new InvalidOperationException("Only clinic appointments can be added to a cabinet.");
        if (!IsAvailable(appointment.StartDate, appointment.EndDate))
            throw new InvalidOperationException("Cabinet is not available at this time.");

        _appointments.Add(appointment);
    }
}