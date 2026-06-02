namespace VetClinic.Domain.Entities;

public class Customer : Person
{
    public decimal LoyaltyPoints { get; private set; }

    public ICollection<Animal> Animals { get; private set; } = new List<Animal>();
    public ICollection<Appointment> Appointments { get; private set; } = new List<Appointment>();

    public Customer() {}

    public Customer(string firstName, string lastName, string? middleName, string email, decimal loyaltyPoints) : base(firstName, lastName, middleName, email)
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
    
    public void GetAnimalById(Guid animalId)
    {
        var animal = Animals.FirstOrDefault(a => a.Id == animalId);
        if (animal == null)
        {
            throw new ArgumentException("Animal with the given id does not exist");
        }
    }

    public void AddAnimal(Animal? animal)
    {
        if (animal == null)
        {
            throw new ArgumentException("Animal can not be null");
        }
        
        Animals.Add(animal);
    }

    public void RemoveAnimalById(Guid animalId)
    { 
        var animal = Animals.FirstOrDefault(a => a.Id == animalId);
        if (animal == null)
        {
            throw new ArgumentException("Animal with the given id does not exist");
        }
        
        Animals.Remove(animal);
    }
    
    public void GetAppointmentById(Guid appointmentId)
    {
        var appointment = Appointments.FirstOrDefault(a => a.Id == appointmentId);
        if (appointment == null)
        {
            throw new ArgumentException("Appointment with the given id does not exist");
        }
    }
    
    public void AddAppointment(Appointment? appointment)
    {
        if (appointment == null)
        {
            throw new ArgumentException("Appointment can not be null");
        }
        
        Appointments.Add(appointment);
    }
    
    public void RemoveAppointmentById(Guid appointmentId)
    { 
        var appointment = Appointments.FirstOrDefault(a => a.Id == appointmentId);
        if (appointment == null)
        {
            throw new ArgumentException("Appointment with the given id does not exist");
        }
        
        Appointments.Remove(appointment);
    }
}