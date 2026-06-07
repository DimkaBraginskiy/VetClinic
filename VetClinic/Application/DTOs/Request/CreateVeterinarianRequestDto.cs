namespace VetClinic.Application.DTOs.Request;

public class CreateVeterinarianRequestDto
{
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    
    public string Type { get; set; }
    public decimal salary { get; set; }
    public DateTime EmploymentDate { get; set; }
    public ICollection<TreatmentOfferringRequestDto> Offerings { get; set; } = new List<TreatmentOfferringRequestDto>();
    public Guid ClinicId { get; set; }
}