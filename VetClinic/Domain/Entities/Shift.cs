namespace VetClinic.Domain.Entities;

public class Shift
{
    public Guid Id { get; private set; }
    
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
}