using Microsoft.Extensions.Options;

namespace VetClinic.Domain.Entities;

public class Veterinarian : Person
{
    public VeterinarianType Type { get; private set; }
    public decimal Salary { get; private set; }
    public DateTime EmploymentDate { get; private set; }

    public ICollection<Shift> Shifts = new List<Shift>();
    public ICollection<Treatment> Treatments = new List<Treatment>();
    public ICollection<Appointment> Appointments = new List<Appointment>();
    public ICollection<Clinic> Clinics = new List<Clinic>();

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
        if (treatment.Type == TreatmentType.Surgery && Type != VeterinarianType.Surgeon)
        {
            throw new InvalidOperationException("Only a surgeon can perform a surgery");
        }
        
        Treatments.Add(treatment);
    }
}