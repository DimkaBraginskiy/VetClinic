namespace VetClinic.Domain.Entities;

public class ClinicAppointment : Appointment
{
    public DateTime ArriveTime { get; private set; }
}