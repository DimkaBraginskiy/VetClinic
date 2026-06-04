using VetClinic.Domain.Enums;

namespace VetClinic.Domain.Entities;

public class Animal
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Name { get; private set; } = null!;
    public DateTime DateOfBirth { get; private set; }
    public decimal? Weight { get; private set; }
    public AnimalSpecies? Species { get; private set; }
    public string? Breed { get; private set; }
    
    public Guid CustomerId { get; private set; }
    public Customer Customer { get; private set; } = null!;

    protected Animal() { }

    public Animal(string name, DateTime dateOfBirth, decimal? weight,
        AnimalSpecies? species, string? breed, Customer customer)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.");
        if (weight is <= 0)
            throw new ArgumentException("Weight must be positive.");
        
        if (species.HasValue != !string.IsNullOrWhiteSpace(breed))
            throw new ArgumentException("Species and breed must both be set or both omitted.");

        Name = name;
        DateOfBirth = dateOfBirth;
        Weight = weight;
        Species = species;
        Breed = breed;
        CustomerId = customer.Id;
        Customer = customer;
    }
}