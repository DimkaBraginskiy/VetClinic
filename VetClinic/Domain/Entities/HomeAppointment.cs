using VetClinic.Domain.Enums;

namespace VetClinic.Domain.Entities;

public class HomeAppointment : Appointment
{
    public Address Address { get; private set; }

    protected HomeAppointment() { }

    public HomeAppointment(
        AppointmentType type,
        DateTime startDate,
        decimal basePrice,
        Guid customerId,
        Guid veterinarianId,
        Address address,
        Treatment? treatment = null)
        : base(type, startDate, basePrice, customerId, veterinarianId, treatment)
    {
        ArgumentNullException.ThrowIfNull(address); // address.Equals(null) throws if address is null

        Address = address;
    }
}