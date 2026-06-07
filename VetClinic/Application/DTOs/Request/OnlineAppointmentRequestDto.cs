namespace VetClinic.Application.DTOs.Request;

public class OnlineAppointmentRequestDto
{
    public Guid VeterinarianId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid AnimalId { get; set; }
    public DateTime StartDate { get; set; }
    public decimal BasePrice { get; set; }
    public string? PromoCode { get; set; }
    public decimal LoyaltyPointsToRedeem { get; set; } = 0;
}