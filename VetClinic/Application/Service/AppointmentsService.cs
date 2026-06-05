using Microsoft.EntityFrameworkCore;
using VetClinic.Application.DTOs;
using VetClinic.Application.DTOs.Request;
using VetClinic.Application.Mapper;
using VetClinic.Application.Services;
using VetClinic.Domain.Entities;
using VetClinic.Domain.Enums;
using VetClinic.Infrastructure;

namespace VetClinic.Application.Service;

public class AppointmentsService : IAppointmentService
{
    private readonly AppDbContext _context;

    public AppointmentsService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ScheduledAppointmentResponseDto> ScheduleOnlineAsync(OnlineAppointmentRequestDto dto)
    {
        // Load vet with shifts + existing appointments so domain validation works
        var vet = await _context.Veterinarians
            .Include(v => v.Shifts)
            .Include(v => v.Appointments)
            .FirstOrDefaultAsync(v => v.Id == dto.VeterinarianId)
            ?? throw new KeyNotFoundException("Veterinarian not found.");

        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == dto.CustomerId)
            ?? throw new KeyNotFoundException("Customer not found.");

        var animal = await _context.Animals
            .FirstOrDefaultAsync(a => a.Id == dto.AnimalId && a.CustomerId == dto.CustomerId)
            ?? throw new KeyNotFoundException("Animal not found or does not belong to this customer.");

        Discount? discount = null;
        if (!string.IsNullOrWhiteSpace(dto.PromoCode))
        {
            var code = dto.PromoCode.Trim().ToUpper();
            discount = await _context.Discounts
                .FirstOrDefaultAsync(d => d.PromoCode == code)
                ?? throw new KeyNotFoundException($"Promo code '{dto.PromoCode}' not found.");
        }

        var appointment = Appointment.CreateOnline(
            AppointmentType.Consultation,
            dto.StartDate,
            dto.BasePrice,
            customer,
            vet,
            animal);

        if (discount != null)
            appointment.ApplyDiscount(discount);

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        return AppointmentMapper.ToResponseDto(appointment);
    }
}