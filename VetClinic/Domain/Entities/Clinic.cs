namespace VetClinic.Domain.Entities;

public class Clinic
{
    public Address Address { get; private set; }
    
    private class Cabinet
    {
        public int Floor { get; private set; }
        public int Number { get; private set; }
    }
}