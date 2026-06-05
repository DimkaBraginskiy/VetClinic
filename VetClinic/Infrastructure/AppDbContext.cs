using Microsoft.EntityFrameworkCore;
using VetClinic.Domain.Entities;

namespace VetClinic.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Customer>     Customers     => Set<Customer>();
    public DbSet<Veterinarian> Veterinarians => Set<Veterinarian>();
    public DbSet<Clinic>       Clinics       => Set<Clinic>();
    public DbSet<Cabinet>      Cabinets      => Set<Cabinet>();
    public DbSet<Animal>       Animals       => Set<Animal>();
    public DbSet<Appointment>  Appointments  => Set<Appointment>();
    public DbSet<Treatment>    Treatments    => Set<Treatment>();
    public DbSet<Shift>        Shifts        => Set<Shift>();
    public DbSet<Discount>     Discounts     => Set<Discount>();

    protected override void OnModelCreating(ModelBuilder model)
    {
    }
}
