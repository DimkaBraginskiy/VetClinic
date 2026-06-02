namespace VetClinic.Domain.Entities;

public class OnlineAppointment : Appointment
{
    public string MeetingLink { get; private set; } = null!;
}