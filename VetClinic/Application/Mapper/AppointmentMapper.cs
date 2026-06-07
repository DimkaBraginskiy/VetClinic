using VetClinic.Application.DTOs;
using VetClinic.Domain.Entities;

namespace VetClinic.Application.Mapper;

public static class AppointmentMapper
{
    public static AppointmentMimimalResponseDto ToMinimalDto(Appointment appt) => new()
    {
        Id               = appt.Id,
        Status           = appt.Status.ToString(),
        Mode             = appt.Mode.ToString(),
        StartDate        = appt.StartDate,
        TotalPrice       = appt.GetTotalPrice(),
        AnimalName       = appt.Animals.FirstOrDefault()?.Name ?? "Unknown",
        VeterinarianName = appt.Veterinarian.GetFullName(),
        TreatmentType    = appt.Treatment?.Type.ToString()
    };

    public static ScheduledAppointmentResponseDto ToResponseDto(Appointment appt) =>
        new()
        {
            Id = appt.Id,
            Status = appt.Status,
            Type = appt.Type,
            Mode = appt.Mode,
            StartDate = appt.StartDate,
            EndDate = appt.EndDate,
            BasePrice = appt.BasePrice,
            TotalPrice = appt.GetTotalPrice(),
            MeetingLink = appt.MeetingLink,
            ArriveTime = appt.ArriveTime,
            CabinetNumber = appt.Cabinet?.Number,
            HomeAddress = appt.HomeAddress == null ? null : new AddressResponseDto(
                appt.HomeAddress.Country,
                appt.HomeAddress.City,
                appt.HomeAddress.Street,
                appt.HomeAddress.Building,
                appt.HomeAddress.Flat,
                appt.HomeAddress.PostalCode),
            Veterinarian = new AppointmentVetResponseDto(
                appt.Veterinarian.Id,
                appt.Veterinarian.GetFullName(),
                appt.Veterinarian.Type.ToString(),
                appt.Veterinarian.ClinicVetId),
            Animals = appt.Animals.Select(a => new AppointmentAnimalResponseDto(
                a.Id,
                a.Name,
                a.Species?.ToString(),
                a.Breed)).ToList(),
            Treatment = appt.Treatment == null ? null : new AppointmentTreatmentResponseDto(
                appt.Treatment.Id,
                appt.Treatment.Type.ToString(),
                appt.Treatment.Duration,
                appt.Treatment.Price),
            Discounts = appt.Discounts.Select(d => new AppointmentDiscountResponseDto(
                d.Id,
                d.Percentage,
                d.PromoCode)).ToList()
        };
}