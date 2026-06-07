using Microsoft.EntityFrameworkCore;
using VetClinic.Application.DTOs;
using VetClinic.Application.DTOs.Request;
using VetClinic.Application.Services;
using VetClinic.Domain.Entities;
using VetClinic.Domain.Enums;
using VetClinic.Infrastructure;

namespace VetClinic.Application.Service;

public class VeterinariansService : IVeterinariansService
{
    private readonly AppDbContext _context;

    public VeterinariansService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> CreateVeterinarianAsync(CreateVeterinarianRequestDto dto)
    {
        if (!Enum.TryParse<VeterinarianType>(dto.Type, ignoreCase: true, out var vetType))
            throw new ArgumentException($"Invalid veterinarian type '{dto.Type}'. " +
                $"Valid values: {string.Join(", ", Enum.GetNames<VeterinarianType>())}");

        var emailExists = await _context.Veterinarians.AnyAsync(v => v.Email == dto.Email)
                       || await _context.Customers.AnyAsync(c => c.Email == dto.Email);
        if (emailExists)
            throw new InvalidOperationException($"A person with email '{dto.Email}' already exists.");

        var clinic = await _context.Clinics.FindAsync(dto.ClinicId)
            ?? throw new KeyNotFoundException($"Clinic '{dto.ClinicId}' not found.");

        if (dto.Offerings == null || dto.Offerings.Count == 0)
            throw new ArgumentException("Veterinarian must have at least one treatment offering.");

        var offerings = new List<(TreatmentType Type, decimal Price, decimal BaseDuration)>();
        foreach (var o in dto.Offerings)
        {
            if (!Enum.TryParse<TreatmentType>(o.Type, ignoreCase: true, out var treatmentType))
                throw new ArgumentException($"Invalid treatment type '{o.Type}'. " +
                    $"Valid values: {string.Join(", ", Enum.GetNames<TreatmentType>())}");

            offerings.Add((treatmentType, o.Price, o.BaseDuration));
        }

        var vet = new Veterinarian(
            vetType,
            dto.salary,
            dto.EmploymentDate,
            offerings,
            clinic,
            dto.FirstName,
            dto.LastName,
            dto.Email,
            dto.MiddleName);

        _context.Veterinarians.Add(vet);
        await _context.SaveChangesAsync();

        return vet.Id;
    }

    public async Task DeleteVeterinarianAsync(Guid id)
    {
        var vet = await _context.Veterinarians.FindAsync(id)
            ?? throw new KeyNotFoundException($"Veterinarian '{id}' not found.");

        _context.Veterinarians.Remove(vet);
        await _context.SaveChangesAsync();
    }

    public async Task<List<VeterinarianDatesResponseDto>> GetAvailableVeterinariansAsync(
        DateOnly date, Guid? clinicId, string? treatmentType)
    {
        TreatmentType? parsedTreatment = null;
        if (!string.IsNullOrWhiteSpace(treatmentType))
        {
            if (!Enum.TryParse<TreatmentType>(treatmentType, ignoreCase: true, out var tt))
                throw new ArgumentException($"Invalid treatment type '{treatmentType}'. " +
                    $"Valid values: {string.Join(", ", Enum.GetNames<TreatmentType>())}");
            parsedTreatment = tt;
        }

        var selectedDate = date.ToDateTime(TimeOnly.MinValue);

        var query = _context.Veterinarians
            .Include(v => v.TreatmentOfferings)
            .Include(v => v.Shifts)
            .Include(v => v.Appointments)
                .ThenInclude(a => a.Treatment)
            .AsQueryable();

        if (clinicId.HasValue)
            query = query.Where(v => v.ClinicId == clinicId.Value);

        if (parsedTreatment.HasValue)
            query = query.Where(v => v.TreatmentOfferings.Any(o => o.Type == parsedTreatment.Value));

        var vets = await query.ToListAsync();

        var result = new List<VeterinarianDatesResponseDto>();
        var now = DateTime.Now;

        foreach (var vet in vets)
        {
            // Find a shift covering the selected day (optionally within the specific clinic)
            var shift = vet.Shifts
                .Where(s => s.StartTime.Date == selectedDate.Date)
                .Where(s => !clinicId.HasValue || s.ClinicId == clinicId.Value)
                .FirstOrDefault();

            if (shift == null) continue;

            // Existing non-cancelled appointments on that day
            var dayAppointments = vet.Appointments
                .Where(a => a.Status != AppointmentStatus.Cancelled
                         && a.StartDate.Date == selectedDate.Date)
                .ToList();

            // Resolve appointment duration and offering for the requested treatment
            TreatmentOffering? offering = null;
            int durationMinutes = 30; // default for Consultation

            if (parsedTreatment.HasValue)
            {
                offering = vet.TreatmentOfferings.FirstOrDefault(o => o.Type == parsedTreatment.Value);
                if (offering != null)
                    durationMinutes = (int)offering.BaseDuration;
            }

            // Busy intervals: [appt.StartDate, appt.EndDate + 15min break]
            var busyIntervals = dayAppointments
                .Select(a => (start: a.StartDate, end: a.EndDate.AddMinutes(15)))
                .ToList();

            var slots = new List<DateTime>();
            var cursor = AlignToNextQuarterHour(shift.StartTime);

            while (cursor.AddMinutes(durationMinutes) <= shift.EndTime)
            {
                if (cursor > now)
                {
                    var slotEnd = cursor.AddMinutes(durationMinutes);
                    // A candidate [cursor, slotEnd] is invalid if it overlaps with any
                    // busy interval when both are expanded by 15 min:
                    // i.e. cursor < b.end  AND  slotEnd + 15min > b.start
                    var slotEndWithBreak = slotEnd.AddMinutes(15);
                    var blocked = busyIntervals.Any(b =>
                        cursor < b.end && slotEndWithBreak > b.start);

                    if (!blocked)
                        slots.Add(cursor);
                }

                cursor = cursor.AddMinutes(15);
            }

            if (slots.Count == 0) continue;

            result.Add(new VeterinarianDatesResponseDto(
                vet.Id,
                vet.FirstName,
                vet.LastName,
                vet.MiddleName,
                vet.Type.ToString(),
                vet.ClinicVetId,
                durationMinutes,
                offering?.Price,
                offering?.Type.ToString(),
                slots
            ));
        }

        return result;
    }

    private static DateTime AlignToNextQuarterHour(DateTime dt)
    {
        var stripped = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
        var remainder = stripped.Minute % 15;
        return remainder == 0 ? stripped : stripped.AddMinutes(15 - remainder);
    }
}