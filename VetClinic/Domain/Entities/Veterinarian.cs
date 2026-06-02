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
}