using Microsoft.EntityFrameworkCore;
using VetClinic.Application.DTOs;
using VetClinic.Application.Services;
using VetClinic.Infrastructure;

namespace VetClinic.Application.Service;

public class CustomersService : ICustomersService
{
    private readonly AppDbContext _context;

    public CustomersService(AppDbContext context)
    {
        _context = context;
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
                        Id = animal.Id,
                        Name = animal.Name,
                        Weight = animal.Weight,
                        Species = animal.Species.ToString(),
                        Breed = animal.Breed
                    }
                );
        }
        
        return dtoResult;
    }

}