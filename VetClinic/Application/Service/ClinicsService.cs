using Microsoft.EntityFrameworkCore;
using VetClinic.Application.DTOs;
using VetClinic.Application.DTOs.Request;
using VetClinic.Application.Services;
using VetClinic.Domain.Entities;
using VetClinic.Infrastructure;

namespace VetClinic.Application.Service;

public class ClinicsService : IClinicService
{
    private readonly AppDbContext _context;

    public ClinicsService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ICollection<ClinicResponseDto>> GetAllClinicsAsync()
    {
        var clinics = await _context.Clinics
            .Include(c => c.Cabinets)
            .ToListAsync();

        return clinics.Select(c => new ClinicResponseDto(
            c.Id,
            c.Name,
            c.Address.Country,
            c.Address.City,
            c.Address.Street,
            c.Address.Building,
            c.Address.PostalCode ?? string.Empty,
            c.Cabinets.Select(cab => new ClinicCabinetResponseDto(cab.Id, cab.Floor, cab.Number)).ToList()
        )).ToList();
    }

    public async Task<Guid> CreateClinicAsync(CreateClinicRequestDto dto)
    {
        var addressTaken = await _context.Clinics.AnyAsync(c =>
            c.Address.Country  == dto.Country  &&
            c.Address.City     == dto.City     &&
            c.Address.Street   == dto.Street   &&
            c.Address.Building == dto.BuildingNumber);
        if (addressTaken)
            throw new InvalidOperationException("A clinic at this address already exists.");

        var address = new Address(
            dto.Country,
            dto.City,
            dto.Street,
            dto.BuildingNumber,
            null,
            dto.PostalCode);

        var clinic = new Clinic(dto.Name, address, dto.InitialCabinetFloor, dto.InitialCabinetNumber);

        _context.Clinics.Add(clinic);
        await _context.SaveChangesAsync();

        return clinic.Id;
    }

    public async Task<Guid> AddCabinetAsync(Guid clinicId, AddCabinetRequestDto dto)
    {
        var clinic = await _context.Clinics
            .Include(c => c.Cabinets)
            .FirstOrDefaultAsync(c => c.Id == clinicId)
            ?? throw new KeyNotFoundException("Clinic not found.");

        var cabinetExists = await _context.Cabinets.AnyAsync(c =>
            c.ClinicId == clinicId && c.Floor == dto.Floor && c.Number == dto.Number);
        if (cabinetExists)
            throw new InvalidOperationException($"Cabinet {dto.Number} on floor {dto.Floor} already exists in this clinic.");

        var cabinet = clinic.AddCabinet(dto.Floor, dto.Number);

        _context.Cabinets.Add(cabinet);
        await _context.SaveChangesAsync();

        return cabinet.Id;
    }
}