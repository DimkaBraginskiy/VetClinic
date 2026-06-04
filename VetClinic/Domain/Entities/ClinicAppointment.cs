using Microsoft.Extensions.Options;
using VetClinic.Domain.Enums;

namespace VetClinic.Domain.Entities;

public class ClinicAppointment : Appointment
{ 
    public DateTime ArriveTime { get; private set; }
    
    public Guid CabinetId { get; private set; }
    public Cabinet Cabinet { get; private set; } = null!;

    protected ClinicAppointment() { }

    public ClinicAppointment(
        AppointmentType type,
        DateTime startDate,
        decimal basePrice,
        Guid customerId,
        Guid veterinarianId,
        DateTime arriveTime,
        Cabinet cabinet,
        Treatment? treatment = null)
        : base(type, startDate, basePrice, customerId, veterinarianId, treatment)
    {
        if (arriveTime >= startDate)
            throw new ArgumentException("Arrival time must be before the appointment start time.");

        ArriveTime = arriveTime;
        CabinetId = cabinet.Id;
        Cabinet = cabinet;

    }

    public void AssignCabinet(Cabinet cabinet)
    {
        CabinetId = cabinet.Id;
        Cabinet = cabinet;
    }
}