using VetClinic.Domain.Enums;

namespace VetClinic.Domain.Entities;

public class OnlineAppointment : Appointment
{ 
    public string MeetingLink { get; private set; } = null!;

    protected OnlineAppointment() { }

    public OnlineAppointment(
        AppointmentType type,
        DateTime startDate,
        decimal basePrice,
        Guid customerId,
        Guid veterinarianId,
        string? meetingLink = null)  // no Treatment — Online is Consultation only
        : base(type, startDate, basePrice, customerId, veterinarianId, treatment: null)
    {
        if (type == AppointmentType.Treatment)
            throw new ArgumentException("Online appointments can only be Consultations.");

        MeetingLink = meetingLink ?? $"https://meet.vetclinic.com/{Guid.NewGuid()}";
    }
}