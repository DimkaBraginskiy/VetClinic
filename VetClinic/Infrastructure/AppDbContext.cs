using Microsoft.EntityFrameworkCore;
using VetClinic.Domain.Entities;

namespace VetClinic.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Customer>     Customers     => Set<Customer>();
    public DbSet<Veterinarian> Veterinarians => Set<Veterinarian>();
    public DbSet<Animal>       Animals       => Set<Animal>();
    public DbSet<Appointment>  Appointments  => Set<Appointment>();
    public DbSet<Treatment>         Treatments         => Set<Treatment>();
    public DbSet<TreatmentOffering> TreatmentOfferings => Set<TreatmentOffering>();
    public DbSet<Clinic>       Clinics       => Set<Clinic>();
    public DbSet<Cabinet>      Cabinets      => Set<Cabinet>();
    public DbSet<Shift>        Shifts        => Set<Shift>();
    public DbSet<Discount>     Discounts     => Set<Discount>();

    protected override void OnModelCreating(ModelBuilder model)
    {
           
        model.Entity<Person>().ToTable("People");
        model.Entity<Customer>().ToTable("Customers");
        model.Entity<Veterinarian>().ToTable("Veterinarians");

        model.Entity<Animal>()
            .Property(a => a.Species)
            .HasConversion<string>();

        model.Entity<Veterinarian>()
            .Property(v => v.Type)
            .HasConversion<string>();

        model.Entity<Treatment>()
            .Property(t => t.Type)
            .HasConversion<string>();

        model.Entity<Treatment>()
            .Property(t => t.Status)
            .HasConversion<string>();

        model.Entity<TreatmentOffering>()
            .Property(o => o.Type)
            .HasConversion<string>();

        model.Entity<Appointment>()
            .Property(a => a.Status)
            .HasConversion<string>();

        model.Entity<Appointment>()
            .Property(a => a.Type)
            .HasConversion<string>();

        model.Entity<Appointment>()
            .Property(a => a.Mode)
            .HasConversion<string>();

        model.Entity<Customer>()
            .HasMany(c => c.Animals)
            .WithOne(a => a.Customer)
            .HasForeignKey(a => a.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        model.Entity<Customer>()
            .HasMany(c => c.Appointments)
            .WithOne(a => a.Customer)
            .HasForeignKey(a => a.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        model.Entity<Veterinarian>()
            .HasMany(v => v.TreatmentOfferings)
            .WithOne(o => o.Veterinarian)
            .HasForeignKey(o => o.VeterinarianId)
            .OnDelete(DeleteBehavior.Cascade);

        model.Entity<Veterinarian>()
            .HasMany(v => v.Shifts)
            .WithOne(s => s.Veterinarian)
            .HasForeignKey(s => s.VeterinarianId)
            .OnDelete(DeleteBehavior.Cascade);

        model.Entity<Veterinarian>()
            .HasMany(v => v.Treatments)
            .WithOne(t => t.Veterinarian)
            .HasForeignKey(t => t.VeterinarianId)
            .OnDelete(DeleteBehavior.Restrict);

        model.Entity<Veterinarian>()
            .HasMany(v => v.Appointments)
            .WithOne(a => a.Veterinarian)
            .HasForeignKey(a => a.VeterinarianId)
            .OnDelete(DeleteBehavior.Restrict);

        model.Entity<Clinic>()
            .HasMany<Veterinarian>("_veterinarians")
            .WithOne(v => v.Clinic)
            .HasForeignKey(v => v.ClinicId)
            .OnDelete(DeleteBehavior.Restrict);

        model.Entity<Clinic>()
            .HasMany(c => c.Cabinets)
            .WithOne(c => c.Clinic)
            .HasForeignKey(c => c.ClinicId)
            .OnDelete(DeleteBehavior.Cascade);

        model.Entity<Clinic>()
            .HasMany(c => c.Shifts)
            .WithOne(s => s.Clinic)
            .HasForeignKey(s => s.ClinicId)
            .OnDelete(DeleteBehavior.Restrict);

        model.Entity<Treatment>()
            .HasOne(t => t.Appointment)
            .WithOne(a => a.Treatment)
            .HasForeignKey<Treatment>(t => t.AppointmentId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        model.Entity<Appointment>()
            .HasOne(a => a.Cabinet)
            .WithMany(c => c.Appointments)
            .HasForeignKey(a => a.CabinetId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        model.Entity<Appointment>()
            .Ignore(a => a.EndDate);

        model.Entity<Appointment>()
            .HasMany(a => a.Animals)
            .WithMany()
            .UsingEntity("AppointmentAnimals");

        model.Entity<Appointment>()
            .HasMany(a => a.Discounts)
            .WithMany(d => d.Appointments)
            .UsingEntity("AppointmentDiscounts");

        model.Entity<Clinic>()
            .OwnsOne(c => c.Address, addr =>
            {
                addr.Property(a => a.Country).IsRequired().HasMaxLength(100);
                addr.Property(a => a.City).IsRequired().HasMaxLength(100);
                addr.Property(a => a.Street).IsRequired().HasMaxLength(200);
                addr.Property(a => a.Building).IsRequired();
                addr.Property(a => a.Flat);
                addr.Property(a => a.PostalCode).HasMaxLength(20);
            });

        model.Entity<Appointment>()
            .OwnsOne(a => a.HomeAddress, addr =>
            {
                addr.Property(a => a.Country).HasColumnName("HomeAddress_Country").HasMaxLength(100);
                addr.Property(a => a.City).HasColumnName("HomeAddress_City").HasMaxLength(100);
                addr.Property(a => a.Street).HasColumnName("HomeAddress_Street").HasMaxLength(200);
                addr.Property(a => a.Building).HasColumnName("HomeAddress_Building");
                addr.Property(a => a.Flat).HasColumnName("HomeAddress_Flat");
                addr.Property(a => a.PostalCode).HasColumnName("HomeAddress_PostalCode").HasMaxLength(20);
            });
    }
}
