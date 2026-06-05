using VetClinic.Domain.Enums;

namespace VetClinic.Application.DTOs.Request;

public class ClinicAppointmentRequestDto
{
    public Guid VeterinarianId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid AnimalId { get; set; }
    public AppointmentType Type { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime ArriveTime { get; set; }
    public Guid CabinetId { get; set; }
    public decimal BasePrice { get; set; }
    public TreatmentType? TreatmentType { get; set; }
    public string? PromoCode { get; set; }
}