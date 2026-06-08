namespace VetClinic.Application.DTOs.Response;

public class AppointmentMimimalResponseDto
{
    public Guid Id { get; set; }
    public string Status { get; set; } = null!;
    public string Mode { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string AnimalName { get; set; } = null!;
    public string VeterinarianName { get; set; } = null!;
    public string? TreatmentType { get; set; }
}