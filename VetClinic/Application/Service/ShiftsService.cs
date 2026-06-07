using Microsoft.EntityFrameworkCore;
using VetClinic.Application.DTOs.Request;
using VetClinic.Application.Services;
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

        
        shift.Delete();

        _context.Shifts.Remove(shift);
        await _context.SaveChangesAsync();
    }
}