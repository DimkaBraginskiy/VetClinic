using Microsoft.EntityFrameworkCore;
using VetClinic.Application.DTOs;
using VetClinic.Application.DTOs.Request;
using VetClinic.Application.DTOs.Response;
using VetClinic.Application.Interface;
using VetClinic.Application.Mapper;
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

    public async Task<DiscountInfoDto> ValidateDiscountAsync(string promoCode)
    {
        var code     = promoCode.Trim().ToUpper();
        var discount = await _context.Discounts.FirstOrDefaultAsync(d => d.PromoCode == code)
            ?? throw new KeyNotFoundException($"Promo code '{promoCode}' not found.");

        return new DiscountInfoDto(discount.PromoCode, discount.Percentage);
    }

    public async Task<Guid> CancelAppointmentAsync(Guid appointmentId)
    {
        var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == appointmentId)
                          ?? throw new KeyNotFoundException($"Aoppointment with id {appointmentId} not found.");

        bool hasTimeToCancel = appointment.StartDate > DateTime.Now;

        if (!hasTimeToCancel)
        {
            throw new InvalidOperationException("Can not cancel already started apppointment");
        }

        appointment.TransitionTo(AppointmentStatus.Cancelled);
        _context.SaveChangesAsync();
        return appointment.Id;
    }

    public async Task<ScheduledAppointmentResponseDto> ScheduleOnlineAsync(OnlineAppointmentRequestDto dto)
    {
        var vet = await _context.Veterinarians
            .Include(v => v.Shifts)
            .Include(v => v.Appointments)
            .Include(v => v.TreatmentOfferings)
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

        // Online consultations have no dedicated offering price; fall back to vet's cheapest offering
        var basePrice = dto.BasePrice > 0
            ? dto.BasePrice
            : vet.TreatmentOfferings.OrderBy(o => o.Price).FirstOrDefault()?.Price ?? 0m;

        var appointment = Appointment.CreateOnline(
            AppointmentType.Consultation,
            dto.StartDate,
            basePrice,
            customer,
            vet,
            animal);
        
        if (discount != null)
        {
            appointment.ApplyDiscount(discount);
        }

        _context.Appointments.Add(appointment);

        if (dto.LoyaltyPointsToRedeem > 0)
            customer.RedeemPoints(dto.LoyaltyPointsToRedeem);

        await _context.SaveChangesAsync();

        return AppointmentMapper.ToResponseDto(appointment);
    }

    public async Task<ScheduledAppointmentResponseDto> ScheduleHomeAsync(HomeAppointmentRequestDto dto)
    {
        var vet = await _context.Veterinarians
                      .Include(v => v.Shifts)
                      .Include(v => v.Appointments)
                      .Include(v => v.TreatmentOfferings)
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

        var address = new Address(
            dto.Address.Country,
            dto.Address.City,
            dto.Address.Street,
            dto.Address.Building,
            dto.Address.Flat,
            dto.Address.PostalCode);

        Treatment? treatment = null;
        if (dto.Type == AppointmentType.Treatment)
        {
            if (dto.TreatmentType == null)
                throw new ArgumentException("TreatmentType is required for a Treatment appointment.");
            treatment = new Treatment(dto.TreatmentType.Value, vet);
        }

        var appointment = Appointment.CreateHome(
            dto.Type,
            dto.StartDate,
            dto.BasePrice,
            customer,
            vet,
            animal,
            address,
            treatment);
        
        if (discount != null)
        {
            appointment.ApplyDiscount(discount);
        }

        _context.Appointments.Add(appointment);

        if (dto.LoyaltyPointsToRedeem > 0)
            customer.RedeemPoints(dto.LoyaltyPointsToRedeem);

        await _context.SaveChangesAsync();
        
        return AppointmentMapper.ToResponseDto(appointment);
    }

    public async Task<ScheduledAppointmentResponseDto> ScheduleClinicAsync(ClinicAppointmentRequestDto dto)
    {
        var vet = await _context.Veterinarians
                      .Include(v => v.Shifts)
                      .Include(v => v.Appointments)
                      .Include(v => v.TreatmentOfferings)
                      .FirstOrDefaultAsync(v => v.Id == dto.VeterinarianId)
                  ?? throw new KeyNotFoundException("Veterinarian not found.");

        var customer = await _context.Customers
                           .FirstOrDefaultAsync(c => c.Id == dto.CustomerId)
                       ?? throw new KeyNotFoundException("Customer not found.");

        var animal = await _context.Animals
                         .FirstOrDefaultAsync(a => a.Id == dto.AnimalId && a.CustomerId == dto.CustomerId)
                     ?? throw new KeyNotFoundException("Animal not found or does not belong to this customer.");

        var clinic = await _context.Clinics
                         .Include(c => c.Cabinets)
                             .ThenInclude(cab => cab.Appointments)
                                 .ThenInclude(a => a.Treatment)
                         .FirstOrDefaultAsync(c => c.Id == dto.ClinicId)
                     ?? throw new KeyNotFoundException("Clinic not found.");

        Discount? discount = null;
        if (!string.IsNullOrWhiteSpace(dto.PromoCode))
        {
            var code = dto.PromoCode.Trim().ToUpper();
            discount = await _context.Discounts
                           .FirstOrDefaultAsync(d => d.PromoCode == code)
                       ?? throw new KeyNotFoundException($"Promo code '{dto.PromoCode}' not found.");
        }

        Treatment? treatment = null;
        if (dto.Type == AppointmentType.Treatment)
        {
            if (dto.TreatmentType == null)
                throw new ArgumentException("TreatmentType is required for a Treatment appointment.");
            treatment = new Treatment(dto.TreatmentType.Value, vet);
        }

        var duration = treatment != null ? (int)treatment.Duration : 30;
        var endTime  = dto.StartDate.AddMinutes(duration);

        var cabinet = clinic.Cabinets.FirstOrDefault(c => c.IsAvailable(dto.StartDate, endTime))
                      ?? throw new InvalidOperationException(
                             "No cabinet is available at the requested time in this clinic.");

        var appointment = Appointment.CreateClinic(
            dto.Type,
            dto.StartDate,
            dto.BasePrice,
            customer,
            vet,
            animal,
            dto.ArriveTime,
            cabinet,
            treatment);
        
        if (discount != null)
        {
            appointment.ApplyDiscount(discount);
        }

        _context.Appointments.Add(appointment);

        if (dto.LoyaltyPointsToRedeem > 0)
            customer.RedeemPoints(dto.LoyaltyPointsToRedeem);

        await _context.SaveChangesAsync();

        return AppointmentMapper.ToResponseDto(appointment);
    }

    public async Task<List<AppointmentMimimalResponseDto>> GetCustomerAppointmentsAsync(Guid customerId, Guid? animalId = null)
    {
        var query = _context.Appointments
            .Where(a => a.CustomerId == customerId)
            .Include(a => a.Veterinarian)
            .Include(a => a.Animals)
            .Include(a => a.Treatment)
            .Include(a => a.Discounts)
            .AsQueryable();

        if (animalId.HasValue)
            query = query.Where(a => a.Animals.Any(animal => animal.Id == animalId.Value));

        var appointments = await query
            .OrderByDescending(a => a.StartDate)
            .ToListAsync();

        return appointments.Select(AppointmentMapper.ToMinimalDto).ToList();
    }

    public async Task<ScheduledAppointmentResponseDto> GetAppointmentByIdAsync(Guid appointmentId, Guid customerId)
    {
        var appointment = await _context.Appointments
            .Where(a => a.Id == appointmentId && a.CustomerId == customerId)
            .Include(a => a.Veterinarian)
            .Include(a => a.Animals)
            .Include(a => a.Treatment)
            .Include(a => a.Discounts)
            .Include(a => a.Cabinet)
            .FirstOrDefaultAsync()
            ?? throw new KeyNotFoundException("Appointment not found.");

        return AppointmentMapper.ToResponseDto(appointment);
    }
    
    public async Task<List<AppointmentMimimalResponseDto>> GetVeterinarianAppointmentsAsync(Guid veterinarianId)
    {
        var appointments = await _context.Appointments
            .Where(a => a.VeterinarianId == veterinarianId)
            .Include(a => a.Veterinarian)
            .Include(a => a.Animals)
            .Include(a => a.Treatment)
            .Include(a => a.Discounts)
            .OrderByDescending(a => a.StartDate)
            .ToListAsync();

        return appointments.Select(AppointmentMapper.ToMinimalDto).ToList();
    }

    public async Task<ScheduledAppointmentResponseDto> GetVeterinarianAppointmentByIdAsync(Guid appointmentId, Guid veterinarianId)
    {
        var appointment = await _context.Appointments
                              .Where(a => a.Id == appointmentId && a.VeterinarianId == veterinarianId)
                              .Include(a => a.Veterinarian)
                              .Include(a => a.Animals)
                              .Include(a => a.Treatment)
                              .Include(a => a.Discounts)
                              .Include(a => a.Cabinet)
                              .FirstOrDefaultAsync()
                          ?? throw new KeyNotFoundException("Appointment not found.");

        return AppointmentMapper.ToResponseDto(appointment);
    }

    public async Task DeleteAppointmentAsync(Guid appointmentId, Guid customerId)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Treatment)
            .FirstOrDefaultAsync(a => a.Id == appointmentId && a.CustomerId == customerId)
            ?? throw new KeyNotFoundException("Appointment not found.");

        if (appointment.Treatment != null)
            _context.Treatments.Remove(appointment.Treatment);

        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();
    }
}