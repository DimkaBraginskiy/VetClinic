namespace VetClinic.Domain.Entities;

public class Veterinarian : Person
{
    public VeterinarianType Type { get; private set; }
    public decimal Salary { get; private set; }
    public DateTime EmploymentDate { get; private set; }

    public Guid ClinicId { get; private set; }
    public Clinic Clinic { get; private set; } = null!;
    public string ClinicVetId { get; private set; } = null!;

    internal void SetClinicVetId(string clinicVetId) => ClinicVetId = clinicVetId;
    internal void ClearClinic() { ClinicId = Guid.Empty; Clinic = null!; ClinicVetId = null!; }
    
    private readonly List<Shift> _shifts = new();
    private readonly List<TreatmentOffering> _treatmentOfferings = new();
    private readonly List<Treatment> _treatments = new();
    private readonly List<Appointment> _appointments = new();
    public IReadOnlyCollection<TreatmentOffering> TreatmentOfferings => _treatmentOfferings.AsReadOnly();
    public IReadOnlyCollection<Shift> Shifts => _shifts.AsReadOnly();
    public IReadOnlyCollection<Treatment> Treatments => _treatments.AsReadOnly();
    public IReadOnlyCollection<Appointment> Appointments =>
        _appointments.OrderBy(a => a.StartDate).ToList().AsReadOnly();

    protected Veterinarian() { }

    public Veterinarian(
        VeterinarianType type,
        decimal salary,
        DateTime employmentDate,
        List<(TreatmentType Type, decimal Price, decimal BaseDuration)> offerings,
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
        if (offerings.Count == 0)
            throw new ArgumentException("Veterinarian must offer at least one treatment.");
        ArgumentNullException.ThrowIfNull(clinic);

        Type = type;
        Salary = salary;
        EmploymentDate = employmentDate;
        ClinicId = clinic.Id;
        Clinic = clinic;
        clinic.AddVeterinarian(this);

        foreach (var (t, price, duration) in offerings)
            AddOffering(t, price, duration);
    }

    public void AddOffering(TreatmentType type, decimal price, decimal baseDuration)
    {
        if (_treatmentOfferings.Any(o => o.Type == type))
            throw new InvalidOperationException($"{type} is already in offerings.");

        _treatmentOfferings.Add(new TreatmentOffering(type, price, baseDuration, this));
    }

    public void RemoveOffering(TreatmentType type)
    {
        if (_treatmentOfferings.Count == 1)
            throw new InvalidOperationException("Veterinarian must have at least one offering.");

        var offering = _treatmentOfferings.FirstOrDefault(o => o.Type == type)
            ?? throw new ArgumentException($"{type} not found in offerings.");

        _treatmentOfferings.Remove(offering);
    }

    public TreatmentOffering? GetOffering(TreatmentType type) =>
        _treatmentOfferings.FirstOrDefault(o => o.Type == type);

    public bool CanPerform(TreatmentType type) =>
        _treatmentOfferings.Any(o => o.Type == type);

    internal void AddTreatment(Treatment treatment)
    {
        _treatments.Add(treatment);
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