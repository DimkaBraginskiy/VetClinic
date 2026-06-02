using VetClinic.Domain.Enums;

namespace VetClinic.Domain.Entities;

public class Animal
{
    public Guid Id { get; private set; }
    
    public string Name { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public decimal Weight { get; private set; }
    
    public AnimalSpecies Species { get; private set; }
    public string Breed { get; private set; }
}