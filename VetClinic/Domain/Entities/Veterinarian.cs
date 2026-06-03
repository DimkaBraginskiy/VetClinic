using Microsoft.Extensions.Options;

namespace VetClinic.Domain.Entities;

public class Veterinarian : Person
{
    public VeterinarianType Type { get; private set; }
    public decimal Salary { get; private set; }
    public DateTime EmploymentDate { get; private set; }    
    
    public Clinic Clinic { get; private set; }
    public Guid ClinicId { get; private set; }
    
    private readonly List<Shift> _shifts = new();
    private readonly List<Treatment> _treatments = new();
    private readonly List<Appointment> _appointments = new();
    
    public IReadOnlyCollection<Shift> Shifts => _shifts.AsReadOnly();
    public IReadOnlyCollection<Treatment> Treatments => _treatments.AsReadOnly();
    public IReadOnlyCollection<Appointment> Appointments => _appointments.AsReadOnly();

    public Veterinarian(
        VeterinarianType type,
        decimal salary,
        DateTime employmentDate, 
        List<Treatment> treatments,
        string firstName,
        string lastName,
        string email,
        string? middleName = null) : base(firstName, lastName, email, middleName)
    {
        Validate(type, salary, employmentDate, treatments);
        Type = type;
        Salary = salary;
        EmploymentDate = employmentDate;

        foreach (var treatment in treatments)
        {
            AddTreatment(treatment);
        }
    }

    private void Validate(VeterinarianType type, decimal salary, DateTime employmentDate, List<Treatment> treatments)
    {
        if (!Enum.IsDefined(type))
        {
            throw new ArgumentException("Invalid veterinarian type");
        }

        if (salary <= 0)
        {
            throw new ArgumentException("Salary can not be smaller or equal 0");
        }

        if (employmentDate > DateTime.Now)
        {
            throw new ArgumentException("Employment date can not be in the future");
        }

        if (treatments.Count == 0)
        {
            throw new ArgumentException("A Veterinarian can not have 0 treatments");
        }
    }

    public void AddTreatment(Treatment treatment)
    {
        if (treatment.Type is TreatmentType.Surgery or TreatmentType.Chiropractic && Type != VeterinarianType.Surgeon){
            throw new InvalidOperationException("Only a surgeon can perform a surgery");
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
        var shiftOverlap = Shifts.FirstOrDefault(s => s.StartTime.Day.Equals(shift.StartTime.Day));
        if (shiftOverlap != null)
        {
            throw new InvalidOperationException("A veterinarian can not have 2 shifts on the same day");
        }
     
        _shifts.Add(shift);
    }

    public void RemoveShift(Shift shift)
    {
        var res = Shifts.Where(s => s.Equals(shift));
        if (!res.Any())
        {
            throw new ArgumentException("Shift with the given parameters does not exist");
        }

        _shifts.Remove(shift);
    }
}