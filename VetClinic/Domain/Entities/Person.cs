namespace VetClinic.Domain.Entities;

public abstract class Person
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string? MiddleName { get; private set; }
    public string Email { get; private set; }

    protected Person() { }

    protected Person(string firstName, string lastName, string email, string? middleName = null)
    {
        Validate(firstName, lastName, email);
        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;
        Email = email;
    }

    private void Validate(string firstName, string lastName, string email)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException("First name can not be null or white space");
        }
        
        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("Last name can not be null or white space");
        }
        
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email can not be null or white space");
        }
    }

    public string GetFullName()
    {
        return string.IsNullOrWhiteSpace(MiddleName)
            ? FirstName + " " + LastName
            : FirstName + " " + MiddleName + " " + LastName;
    }
}