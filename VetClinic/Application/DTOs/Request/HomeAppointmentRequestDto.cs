using VetClinic.Domain.Enums;

namespace VetClinic.Application.DTOs.Request;

public class HomeAppointmentRequestDto
{
    public Guid VeterinarianId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid AnimalId { get; set; }
    public AppointmentType Type { get; set; }
    public DateTime StartDate { get; set; }
    public decimal BasePrice { get; set; }
    public AddressRequestDto Address { get; set; } = null!;
    public TreatmentType? TreatmentType { get; set; }
    public string? PromoCode { get; set; }
    public decimal LoyaltyPointsToRedeem { get; set; } = 0;
}