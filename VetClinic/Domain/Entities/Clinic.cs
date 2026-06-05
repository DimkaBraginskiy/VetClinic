namespace VetClinic.Domain.Entities;

public class Clinic
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Address Address { get; private set; } = null!;

    private readonly List<Veterinarian> _veterinarians = new();
    
    private readonly List<Cabinet> _cabinets = new();
    private readonly List<Shift> _shifts = new();

    public IReadOnlyDictionary<string, Veterinarian> Veterinarians =>
        _veterinarians.ToDictionary(v => v.ClinicVetId).AsReadOnly();
    public IReadOnlyCollection<Cabinet> Cabinets => _cabinets.AsReadOnly();
    public IReadOnlyCollection<Shift> Shifts => _shifts.AsReadOnly();
    
    protected Clinic() { }

    public Clinic(Address address, int initialFloor, int initialNumber)
    {
        ArgumentNullException.ThrowIfNull(address);
        Address = address;
        AddCabinet(initialFloor, initialNumber);
    }

    public Cabinet AddCabinet(int floor, int number)
    {
        var cabinet = new Cabinet(floor, number, this);
        _cabinets.Add(cabinet);
        return cabinet;
    }
    
    public string AddVeterinarian(Veterinarian veterinarian)
    {
        ArgumentNullException.ThrowIfNull(veterinarian);
        if (_veterinarians.Any(v => v.Id == veterinarian.Id))
            throw new InvalidOperationException("Veterinarian already registered in this clinic.");

        var clinicVetId = $"VET-{Id.ToString()[..8].ToUpper()}-{_veterinarians.Count + 1:D3}";
        veterinarian.SetClinicVetId(clinicVetId);
        _veterinarians.Add(veterinarian);
        return clinicVetId;
    }

    public Veterinarian? GetVeterinarianByClinicVetId(string clinicVetId)
        => _veterinarians.FirstOrDefault(v => v.ClinicVetId == clinicVetId);
    
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