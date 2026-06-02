namespace VetClinic.Domain.Entities;

public class HomeAppointment : Appointment
{
    public Address Address { get; private set; }
}