using Microsoft.Extensions.Options;

namespace VetClinic.Domain.Entities;

public class Clinic
{
    public Guid Id { get; private set; }
    
    public Address Address { get; private set; }

    private readonly Dictionary<Guid, Veterinarian> _veterinarians = new();
    public IReadOnlyDictionary<Guid, Veterinarian> Veterinarians => _veterinarians.AsReadOnly();

    private readonly List<Cabinet> _cabinets = new();
    public IReadOnlyCollection<Cabinet> Cabinets => _cabinets.AsReadOnly();

    public Clinic(Address address, List<Cabinet> cabinets, List<Veterinarian> veterinarians)
    {
        Validate(address, cabinets, veterinarians);
        Address = address;

        foreach (var cabinet in cabinets)
        {
            AddCabinet(cabinet);
        }

        foreach (var veterinarian in veterinarians)
        {
            AddVeterinarian(veterinarian);
        }
    }
    
    private void Validate(Address address, List<Cabinet> cabinets, List<Veterinarian> veterinarians)
    {
        if (address == null)
        {
            throw new ArgumentException("Address can not be null");
        }

        if (cabinets.Count == 0)
        {
            throw new ArgumentException("Clinic must have at least one cabinet");
        }

        if (veterinarians.Count == 0)
        {
            throw new ArgumentException("Clinic must have at least one veterinarian");
        }
    }
    
    public void AddVeterinarian(Veterinarian veterinarian) 
    { 
        if (veterinarian == null) 
        { 
            throw new ArgumentException("Veterinarian can not be null");
        }

        if (_veterinarians.ContainsKey(veterinarian.Id)) 
        { 
            throw new ArgumentException("Veterinarian with the given id already exists");
        }
        
        _veterinarians.Add(veterinarian.Id, veterinarian);
    }
    public void AddCabinet(Cabinet cabinet) 
    { 
        if (cabinet == null) 
        { 
            throw new ArgumentException("Cabinet can not be null");
        }
        
        _cabinets.Add(cabinet);
    }


    public class Cabinet
    {
        public Guid Id { get; private set; }
        
        public int Floor { get; private set; }
        public int Number { get; private set; }
        
        public Clinic Clinic { get; private set; }
        public Guid ClinicId { get; private set; }
        
        private readonly List<ClinicAppointment> _appointments = new();
        public IReadOnlyCollection<ClinicAppointment> Appointments =>   
            _appointments.AsReadOnly();
        
        public Cabinet(int floor, int number, Clinic clinic)
        {
            Validate(floor, number, clinic);
            Floor = floor;
            Number = number;
            Clinic = clinic;
        }
        
        private void Validate(int floor, int number, Clinic clinic)
        {
            if (floor < 0)
            {
                throw new ArgumentException("Floor can not be negative");
            }

            if (number <= 0)
            {
                throw new ArgumentException("Number must be greater than 0");
            }

            if (clinic == null)
            {
                throw new ArgumentException("Clinic can not be null");
            }
        }
        
        public bool IsAvailable(DateTime time)
        {
            return !_appointments.Any(a =>
                a.Status != AppointmentStatus.Cancelled &&
                a.StartDate <= time);
        }

        public void AddAppointment(ClinicAppointment appointment)       
        {
            if (!IsAvailable(appointment.StartDate))
                throw new InvalidOperationException("Cabinet is not available at this time.");

            _appointments.Add(appointment);
        }
    }
}