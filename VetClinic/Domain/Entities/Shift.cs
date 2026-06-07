namespace VetClinic.Domain.Entities;

public class Shift
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    
    public Guid VeterinarianId { get; private set; }
    public Veterinarian Veterinarian { get; private set; } = null!;
    
    public Guid ClinicId { get; private set; }
    public Clinic Clinic { get; private set; } = null!; 
    protected Shift() { }

    private Shift(DateTime startTime, DateTime endTime, Veterinarian veterinarian, Clinic clinic)
    {
        if (startTime >= endTime)
        {
            throw new ArgumentException("Start time must be earlier than end time.");
        }

        if (startTime < DateTime.Now)
        {
            throw new ArgumentException("Shift can not start in the past.");
        }

        if (veterinarian.ClinicId != clinic.Id)
        {
            throw new InvalidOperationException(
                "Can not create shift: the veterinarian does not work in a specified clinic.");
        }

        StartTime = startTime;
        EndTime = endTime;
        VeterinarianId = veterinarian.Id;
        Veterinarian = veterinarian;
        ClinicId = clinic.Id;
        Clinic = clinic;
    }

    public static Shift Create(DateTime startTime, DateTime endTime, Veterinarian veterinarian, Clinic clinic)
    {
        if (veterinarian == null)
            throw new ArgumentException("Veterinarian can not be null");
        if (clinic == null)
            throw new ArgumentException("Clinic can not be null");
        
        var shift = new Shift(startTime, endTime, veterinarian, clinic);
        
        veterinarian.AddShift(shift);
        clinic.AddShift(shift);
        
        return shift;
    }
    
    public void Delete()
    {
        Veterinarian.RemoveShift(this);
        Clinic.RemoveShift(this);

        Veterinarian = null!;
        Clinic = null!;
    }
}