using VetClinic.Domain.Enums;

namespace VetClinic.Application.DTOs;

public class ScheduledAppointmentResponseDto
{
    public Guid Id { get; set; }
    public AppointmentStatus Status { get; set; }
    public AppointmentType Type { get; set; }
    public AppointmentMode Mode { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal BasePrice { get; set; }
    public decimal TotalPrice { get; set; }
    
    public string? MeetingLink { get; set; } // Online 
    
    public AddressResponseDto? HomeAddress { get; set; }    // Home
    
    public DateTime? ArriveTime { get; set; }       // Clinic
    public int? CabinetNumber { get; set; }         // Clinic
    
    public AppointmentVetResponseDto Veterinarian { get; set; } = null!;
    public List<AppointmentAnimalResponseDto> Animals { get; set; } = new();
    public AppointmentTreatmentResponseDto? Treatment { get; set; }
    public List<AppointmentDiscountResponseDto> Discounts { get; set; } = new();
}