namespace VetClinic.Domain.Entities;

public class Veterinarian : Person
{
    public VeterinarianType Type { get; private set; }
    public decimal Salary { get; private set; }
    public DateTime EmploymentDate { get; private set; }

    public Guid ClinicId { get; private set; }
    public Clinic Clinic { get; private set; } = null!;
    
    private readonly List<Shift> _shifts = new();
    private readonly List<TreatmentType> _availableTreatmentTypes = new();
    private readonly List<Treatment> _treatments = new();
    private readonly List<Appointment> _appointments = new();
    public IReadOnlyCollection<TreatmentType> AvailableTreatmentTypes => _availableTreatmentTypes.AsReadOnly();
    public IReadOnlyCollection<Shift> Shifts => _shifts.AsReadOnly();
    public IReadOnlyCollection<Treatment> Treatments => _treatments.AsReadOnly();
    public IReadOnlyCollection<Appointment> Appointments => 
        _appointments.OrderBy(a => a.StartDate).ToList().AsReadOnly();
    
    protected Veterinarian() { }

    public Veterinarian(
        VeterinarianType type,
        decimal salary,
        DateTime employmentDate,
        List<TreatmentType> availableTreatmentTypes,
        Clinic clinic,
        string firstName, string lastName, string email,
        string? middleName = null) : base(firstName, lastName, email, middleName)
    {
        if (!Enum.IsDefined(type))
            throw new ArgumentException("Invalid veterinarian type.");
        if (salary <= 0)
            throw new ArgumentException("Salary must be positive.");
        if (employmentDate > DateTime.Now)
            throw new ArgumentException("Employment date cannot be in the future.");
        if (availableTreatmentTypes.Count == 0)
            throw new ArgumentException("Veterinarian must offer at least one treatment.");
        ArgumentNullException.ThrowIfNull(clinic);

        Type = type;
        Salary = salary;
        EmploymentDate = employmentDate;
        ClinicId = clinic.Id;
        Clinic = clinic;
        clinic.AddVeterinarian(this);

        foreach (var t in availableTreatmentTypes)
            AddAvailableTreatment(t);
    }

    public void AddAvailableTreatment(TreatmentType type)
    {
        if (type is TreatmentType.Surgery or TreatmentType.Chiropractic
            && Type != VeterinarianType.Surgeon)
            throw new InvalidOperationException("Only a Surgeon can perform Surgery or Chiropractic.");

        if (_availableTreatmentTypes.Contains(type))
            throw new InvalidOperationException($"{type} already in offerings.");

        _availableTreatmentTypes.Add(type);
    }
    
    public void RemoveAvailableTreatment(TreatmentType type)
    {
        if (_availableTreatmentTypes.Count == 1)
            throw new InvalidOperationException("Veterinarian must offer at least one treatment.");
        if (!_availableTreatmentTypes.Remove(type))
            throw new ArgumentException($"{type} not found in offerings.");
    }

    public bool CanPerform(TreatmentType type) => _availableTreatmentTypes.Contains(type);

    public void AddTreatment(Treatment treatment)
    {
        if (treatment.Type is TreatmentType.Surgery or TreatmentType.Chiropractic && Type != VeterinarianType.Surgeon){
            throw new InvalidOperationException("Only a surgeon can perform a surgery");
        }
        if(Treatments.Any(t => t.Type == treatment.Type))
        {
            throw new InvalidOperationException($"Treatment {treatment} already assigned");
        }

        _treatments.Add(treatment);
    }

    public void RemoveTreatment(TreatmentType treatmentType)
    {
        if (Treatments.Count == 1)
            throw new InvalidOperationException("Veterinarian must have at least one treatment.");
        
        var treatment = Treatments.FirstOrDefault(t => t.Type.Equals(treatmentType));
        if (treatment == null)
        {
            throw new ArgumentException("Treatment with the given type does not exist");
        }

        _treatments.Remove(treatment);
    }

    public void AddShift(Shift shift)
    {
        if (Shifts.Any(s => s.StartTime.Date == shift.StartTime.Date))
        {
            throw new InvalidOperationException("A veterinarian can not have 2 shifts on the same day");
        }
     
        _shifts.Add(shift);
    }

    public void RemoveShift(Shift shift)
    {
        var found = _shifts.FirstOrDefault(s => s.Id == shift.Id)
                    ?? throw new ArgumentException("Shift not found.");
        _shifts.Remove(found);
    }

    public void AddAppointment(Appointment appointment)
    {
        if (appointment == null)
        {
            throw new ArgumentNullException(nameof(appointment));
        }

        ValidateAppointment(appointment);
        
        if (_appointments.Any(a => a.Id == appointment.Id))
            throw new InvalidOperationException("This appointment is already assigned.");
        
        _appointments.Add(appointment);
    }

    private void ValidateAppointment(Appointment appointment)
    {
        var hasShiftDuringDay = _shifts.Any(s => 
            s.StartTime.Date == appointment.StartDate.Date);

        if (!hasShiftDuringDay)
        {
            throw new InvalidOperationException(
                $"Veterinarian has no shift on {appointment.StartDate.Date:yyyy-m-dd}. Can not assign shift");
        }

        var hasOverlap = _appointments.Any(a =>
            a.Status != AppointmentStatus.Cancelled && HasTimeOverlap(a, appointment));

        if (hasOverlap)
        {
            throw new InvalidOperationException("Appointment which you want to create overlaps with existing one");
        }
    }

    private bool HasTimeOverlap(Appointment existingAppointment, Appointment newAppointment)
    {
        return newAppointment.StartDate < existingAppointment.EndDate &&
               newAppointment.EndDate > existingAppointment.StartDate;
    }
    
    public void RemoveAppointment(Guid appointmentId)
    {
        var appt = _appointments.FirstOrDefault(a => a.Id == appointmentId);
        if (appt == null)
            throw new ArgumentException("Appointment not found.");

        _appointments.Remove(appt);
    }
}