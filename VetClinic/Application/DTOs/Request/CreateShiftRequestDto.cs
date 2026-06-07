namespace VetClinic.Application.DTOs.Request;

public class CreateShiftRequestDto
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public Guid VeterinarianId { get; set; }
    public Guid ClinicId { get; set; }
}