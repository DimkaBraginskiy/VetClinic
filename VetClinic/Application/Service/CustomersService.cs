using Microsoft.EntityFrameworkCore;
using VetClinic.Application.DTOs;
using VetClinic.Application.DTOs.Request;
using VetClinic.Application.DTOs.Response;
using VetClinic.Application.Interface;
using VetClinic.Domain.Entities;
using VetClinic.Domain.Enums;
using VetClinic.Infrastructure;

namespace VetClinic.Application.Service;

public class CustomersService : ICustomersService
{
    private readonly AppDbContext _context;

    public CustomersService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> CreateCustomerAsync(CreateCustomerRequestDto dto)
    {
        var emailExists = await _context.Customers.AnyAsync(c => c.Email == dto.Email)
                       || await _context.Veterinarians.AnyAsync(v => v.Email == dto.Email);
        if (emailExists)
            throw new InvalidOperationException($"A person with email '{dto.Email}' already exists.");

        var customer = new Customer(
            dto.FirstName,
            dto.LastName,
            dto.Email,
            dto.LoyaltyPoints,
            dto.MiddleName);

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return customer.Id;
    }

    public async Task<Guid?> GetCustomerIdByEmailAsync(string email)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Email == email);
        return customer?.Id;
    }

    public async Task<int> GetAppointmentCountAsync(Guid customerId) =>
        await _context.Appointments.CountAsync(a => a.CustomerId == customerId);

    public async Task<decimal> GetLoyaltyPointsAsync(Guid customerId)
    {
        var customer = await _context.Customers.FindAsync(customerId)
            ?? throw new KeyNotFoundException($"Customer with ID {customerId} not found.");
        return customer.LoyaltyPoints;
    }

    public async Task DeleteCustomerAnimalAsync(Guid customerId, Guid animalId)
    {
        var animal = await _context.Animals
            .FirstOrDefaultAsync(a => a.Id == animalId && a.CustomerId == customerId)
            ?? throw new KeyNotFoundException("Animal not found or does not belong to this customer.");

        _context.Animals.Remove(animal);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<CustomerAnimalResponseDto>> GetCustomerAnimalsAsync(Guid customerId)
    {
        var customer = await _context.Customers.FindAsync(customerId);
        if (customer == null) throw new KeyNotFoundException($"Customer with ID {customerId} not found.");

        var animals = await _context.Animals
            .Where(a => a.CustomerId == customerId)
            .ToListAsync();
        
        var dtoResult = new List<CustomerAnimalResponseDto>();
        
        foreach (var animal in animals)
        {
            dtoResult.Add(
                    new CustomerAnimalResponseDto
                    {
                        Id          = animal.Id,
                        Name        = animal.Name,
                        DateOfBirth = animal.DateOfBirth,
                        Weight      = animal.Weight,
                        Species     = animal.Species?.ToString(),
                        Breed       = animal.Breed
                    }
                );
        }
        
        return dtoResult;
    }

    public async Task<CustomerAnimalResponseDto> GetAnimalByIdAsync(Guid customerId, Guid animalId)
    {
        var animal = await _context.Animals
            .FirstOrDefaultAsync(a => a.Id == animalId && a.CustomerId == customerId)
            ?? throw new KeyNotFoundException("Animal not found.");

        return new CustomerAnimalResponseDto
        {
            Id          = animal.Id,
            Name        = animal.Name,
            DateOfBirth = animal.DateOfBirth,
            Weight      = animal.Weight,
            Species     = animal.Species?.ToString(),
            Breed       = animal.Breed
        };
    }

    public async Task<Guid> CreateCustomerAnimalAsync(CreatAnimalRequestDto dto, Guid customerId)
    {
        var customer = await _context.Customers.FindAsync(customerId);
        if (customer == null) throw new KeyNotFoundException($"Customer with ID {customerId} not found.");
        
        var species = Enum.Parse<AnimalSpecies>(dto.Species, true);

        var animal = new Animal(
            dto.Name,
            dto.DateOfBirth,
            dto.Weight,
            species,
            dto.Breed,
            customer
            );
        
        _context.Animals.Add(animal);
        await _context.SaveChangesAsync();
        
        return animal.Id;
    }

}