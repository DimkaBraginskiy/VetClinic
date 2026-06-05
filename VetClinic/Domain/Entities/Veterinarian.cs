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

    public bool IsAvailable(DateTime start, DateTime end)
    {
        var hasCoveringShift = _shifts.Any(s => s.StartTime <= start && s.EndTime >= end);
        if (!hasCoveringShift) return false;

        var hasOverlap = _appointments.Any(a =>
            a.Status != AppointmentStatus.Cancelled &&
            start < a.EndDate && end > a.StartDate);

        return !hasOverlap;
    }

    public void AddShift(Shift shift)
    {
        var hasOverlap = _shifts.Any(s =>
            shift.StartTime < s.EndTime && shift.EndTime > s.StartTime);

        if (hasOverlap)
            throw new InvalidOperationException("Shift overlaps with an existing shift.");

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
        if (!IsAvailable(appointment.StartDate, appointment.EndDate))
            throw new InvalidOperationException(
                $"Veterinarian is not available from {appointment.StartDate:HH:mm} to {appointment.EndDate:HH:mm} " +
                $"on {appointment.StartDate:yyyy-MM-dd}. No covering shift or overlapping appointment.");
    }
    
    public void RemoveAppointment(Guid appointmentId)
    {
        var appt = _appointments.FirstOrDefault(a => a.Id == appointmentId);
        if (appt == null)
            throw new ArgumentException("Appointment not found.");

        _appointments.Remove(appt);
    }
}