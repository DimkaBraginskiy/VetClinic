using Microsoft.EntityFrameworkCore;
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
}