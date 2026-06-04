namespace VetClinic.Domain.Entities;

public class Discount
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public decimal Percentage { get; private set; }
    public string PromoCode { get; private set; } = null!;

    private readonly List<Appointment> _appointments = new();
    public IReadOnlyCollection<Appointment> Appointments => _appointments.AsReadOnly();

    protected Discount() { }

    public Discount(decimal percentage, string promoCode)
    {
        if (percentage <= 0 || percentage > 100)
            throw new ArgumentException("Percentage must be between 0 and 100 percents");
        if (string.IsNullOrWhiteSpace(promoCode))
            throw new ArgumentException("Promo code is required.");

        Percentage = percentage;
        PromoCode = promoCode.Trim().ToUpper();
    }

    internal void AddAppointment(Appointment appointment)
    {
        if(!_appointments.Any(a => a.Id == appointment.Id))
            _appointments.Add(appointment);
    }
}