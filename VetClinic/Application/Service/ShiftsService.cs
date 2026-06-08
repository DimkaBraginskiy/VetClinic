using Microsoft.EntityFrameworkCore;
using VetClinic.Application.DTOs;
using VetClinic.Application.DTOs.Request;
using VetClinic.Application.DTOs.Response;
using VetClinic.Application.Interface;
using VetClinic.Domain.Entities;
using VetClinic.Infrastructure;

namespace VetClinic.Application.Service;

public class ShiftsService : IShiftsService
{
    private readonly AppDbContext _context;

    public ShiftsService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> CreateShiftAsync(CreateShiftRequestDto dto)
    {
        var vet = await _context.Veterinarians
            .Include(v => v.Shifts)
            .FirstOrDefaultAsync(v => v.Id == dto.VeterinarianId)
            ?? throw new KeyNotFoundException($"Veterinarian '{dto.VeterinarianId}' not found.");
        
        var clinic = await _context.Clinics
            .Include("_veterinarians")
            .FirstOrDefaultAsync(c => c.Id == dto.ClinicId)
            ?? throw new KeyNotFoundException($"Clinic '{dto.ClinicId}' not found.");
        
        var shift = Shift.Create(dto.StartTime, dto.EndTime, vet, clinic);

        _context.Shifts.Add(shift);
        await _context.SaveChangesAsync();

        return shift.Id;
    }

    public async Task DeleteShiftAsync(Guid id)
    {
        var shift = await _context.Shifts
            .Include(s => s.Veterinarian)
                .ThenInclude(v => v.Shifts)
            .Include(s => s.Clinic)
                .ThenInclude(c => c.Shifts)
            .FirstOrDefaultAsync(s => s.Id == id)
            ?? throw new KeyNotFoundException($"Shift '{id}' not found.");

        var hasAppointment = await _context.Appointments.AnyAsync(a =>
            a.VeterinarianId == shift.VeterinarianId &&
            a.Status != AppointmentStatus.Cancelled &&
            a.StartDate >= shift.StartTime &&
            a.StartDate < shift.EndTime);

        if (hasAppointment)
            throw new ArgumentException("Cannot remove shift: there is an appointment scheduled during this time :(");

        shift.Delete();
        _context.Shifts.Remove(shift);
        await _context.SaveChangesAsync();
    }

    public async Task<List<ShiftResponseDto>> GetVetShiftsAsync(Guid veterinarianId)
    {
        var shifts = await _context.Shifts
            .Where(s => s.VeterinarianId == veterinarianId)
            .Include(s => s.Clinic)
            .OrderBy(s => s.StartTime)
            .ToListAsync();

        return shifts.Select(s => new ShiftResponseDto(s.Id, s.StartTime, s.EndTime, s.ClinicId, s.Clinic.Name))
                     .ToList();
    }
}