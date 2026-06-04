namespace VetClinic.Domain.Entities;

public class Customer : Person
{ 
    public decimal LoyaltyPoints { get; private set; }

    private readonly List<Animal> _animals = new();
    private readonly List<Appointment> _appointments = new();
    public IReadOnlyCollection<Animal> Animals => _animals.AsReadOnly();
    public IReadOnlyCollection<Appointment> Appointments => _appointments.AsReadOnly();

    public Customer() {}

    public Customer(string firstName, string lastName, string email, decimal loyaltyPoints, string? middleName = null) 
        : base(firstName, lastName, middleName, email)
    {
        Validate(loyaltyPoints);
        LoyaltyPoints = loyaltyPoints;
    }

    private void Validate(decimal loyaltyPoints)
    {
        if (loyaltyPoints < 0)
        {
            throw new ArgumentException("Loyalty points can not be negative");
        }
    }

    public void RedeemPoints(decimal amount)
    {
        if (LoyaltyPoints - amount < 0)
        {
            throw new ArgumentException("Invalid points amount");
        }

        LoyaltyPoints -= amount;
    }
    
    public void EarnPoints(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Invalid points amount");
        }

        LoyaltyPoints += amount;
    }
    
    public Animal GetAnimalById(Guid animalId)
    {
        return _animals.FirstOrDefault(a => a.Id == animalId)
               ?? throw new ArgumentException("Animal not found.");
    }

    public void AddAnimal(Animal? animal)
    {
        if (animal == null)
        {
            throw new ArgumentException("Animal can not be null");
        }
        
        _animals.Add(animal);
    }

    public void RemoveAnimalById(Guid animalId)
    { 
        var animal = Animals.FirstOrDefault(a => a.Id == animalId);
        if (animal == null)
        {
            throw new ArgumentException("Animal with the given id does not exist");
        }
        
        _animals.Remove(animal);
    }
    
    public Appointment GetAppointmentById(Guid appointmentId)
    {
        return _appointments.FirstOrDefault(a => a.Id == appointmentId)
               ?? throw new ArgumentException("Appointment not found.");
    }
    
    public void AddAppointment(Appointment? appointment)
    {
        if (appointment == null)
        {
            throw new ArgumentException("Appointment can not be null");
        }
        
        _appointments.Add(appointment);
    }
    
    public void RemoveAppointmentById(Guid appointmentId)
    { 
        var appointment = Appointments.FirstOrDefault(a => a.Id == appointmentId);
        if (appointment == null)
        {
            throw new ArgumentException("Appointment with the given id does not exist");
        }
        
        _appointments.Remove(appointment);
    }
}