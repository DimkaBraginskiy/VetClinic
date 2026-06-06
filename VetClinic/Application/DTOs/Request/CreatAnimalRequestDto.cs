namespace VetClinic.Application.DTOs.Request;

public class CreatAnimalRequestDto
{
    public string Name { get; set; }
    public DateTime DateOfBirth { get; set; }
    public decimal? Weight { get; set; }
    public string? Species { get; set; }
    public string? Breed { get; set; }
}