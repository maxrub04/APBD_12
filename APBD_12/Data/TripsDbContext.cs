using APBD_12.Models;

namespace APBD_12.Data;

using Microsoft.EntityFrameworkCore;

public class TripsDbContext : DbContext
{
    public TripsDbContext(DbContextOptions<TripsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Client> Clients { get; set; }
    public DbSet<ClientTrip> ClientTrips { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<CountryTrip> CountryTrips { get; set; }
    public DbSet<Trip> Trips { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Client
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.IdClient);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(120);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(120);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(120);
            entity.Property(e => e.Telephone).IsRequired().HasMaxLength(120);
            entity.Property(e => e.Pesel).IsRequired().HasMaxLength(120);
        });

        // Trip
        modelBuilder.Entity<Trip>(entity =>
        {
            entity.HasKey(e => e.IdTrip);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(120);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(220);
            entity.Property(e => e.DateFrom).IsRequired();
            entity.Property(e => e.DateTo).IsRequired();
            entity.Property(e => e.MaxPeople).IsRequired();
        });

        // ClientTrip
        modelBuilder.Entity<ClientTrip>(entity =>
        {
            entity.HasKey(e => new { e.IdClient, e.IdTrip });

            entity.HasOne(d => d.IdClientNavigation)
                .WithMany(p => p.ClientTrips)
                .HasForeignKey(d => d.IdClient)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.IdTripNavigation)
                .WithMany(p => p.ClientTrips)
                .HasForeignKey(d => d.IdTrip)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.RegisteredAt).IsRequired();
            entity.Property(e => e.PaymentDate).IsRequired(false);
        });

        // Country
        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.IdCountry);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(120);
        });

        // CountryTrip
        modelBuilder.Entity<CountryTrip>(entity =>
        {
            entity.HasKey(e => new { e.IdCountry, e.IdTrip });

            entity.HasOne(d => d.IdCountryNavigation)
                .WithMany(p => p.CountryTrips)
                .HasForeignKey(d => d.IdCountry)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.IdTripNavigation)
                .WithMany(p => p.CountryTrips)
                .HasForeignKey(d => d.IdTrip)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
