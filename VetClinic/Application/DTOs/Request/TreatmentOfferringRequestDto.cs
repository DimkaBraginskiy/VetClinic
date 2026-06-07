namespace VetClinic.Application.DTOs.Request;

public class TreatmentOfferringRequestDto
{
    public string Type { get; set; }
    public decimal Price { get; set; }
    public decimal BaseDuration { get; set; }
}