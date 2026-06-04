namespace VetClinic.Domain.Entities;

public class Clinic
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Address Address { get; private set; } = null!;

    private readonly Dictionary<string, Veterinarian> _veterinarians = new();
    private readonly List<Cabinet> _cabinets = new();
    private readonly List<Shift> _shifts = new();
    public IReadOnlyDictionary<string, Veterinarian> Veterinarians => _veterinarians.AsReadOnly();
    public IReadOnlyCollection<Cabinet> Cabinets => _cabinets.AsReadOnly();
    public IReadOnlyCollection<Shift> Shifts => _shifts.AsReadOnly();
    
    protected Clinic() { }

    public Clinic(Address address, List<Cabinet> cabinets, List<Veterinarian> veterinarians)
    {
        Validate(address, cabinets, veterinarians);
        Address = address;

        foreach (var cabinet in cabinets)
        {
            AddCabinet(cabinet.Floor, cabinet.Number);
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
    
    public string AddVeterinarian(Veterinarian veterinarian)
    {
        if (veterinarian == null)
            throw new ArgumentNullException(nameof(veterinarian));
        if (_veterinarians.Values.Any(v => v.Id == veterinarian.Id))
            throw new InvalidOperationException("Veterinarian already registered in this clinic.");

        var clinicVetId = $"VET-{Id.ToString()[..8].ToUpper()}-{_veterinarians.Count + 1:D3}";
        _veterinarians.Add(clinicVetId, veterinarian);
        return clinicVetId; // caller gets the clinic-assigned id back
    }

    public Veterinarian? GetVeterinarianByClinicVetId(string clinicVetId)
        => _veterinarians.TryGetValue(clinicVetId, out var vet) ? vet : null;
    
    public Cabinet AddCabinet(int floor, int number)
    {
        var cabinet = new Cabinet(floor, number, this);
        _cabinets.Add(cabinet);
        return cabinet;
    }
    
    public void RemoveCabinet(Guid cabinetId)
    {
        if (_cabinets.Count == 1)
            throw new InvalidOperationException("Clinic must have at least one cabinet.");

        var cabinet = _cabinets.FirstOrDefault(c => c.Id == cabinetId)
                      ?? throw new ArgumentException("Cabinet not found.");

        _cabinets.Remove(cabinet);
    }
    
    public void AddShift(Shift shift)
    {
        if (_shifts.Any(s => s.Id == shift.Id)) return;
        _shifts.Add(shift);
    }

    public void RemoveShift(Shift shift)
    {
        _shifts.Remove(shift);
    }
}