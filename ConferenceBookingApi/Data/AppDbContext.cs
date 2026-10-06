using Microsoft.EntityFrameworkCore;
using ConferenceBookingApi.Models;

namespace ConferenceBookingApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Hall> Halls => Set<Hall>();
    public DbSet<ServiceItem> Services => Set<ServiceItem>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Каскадне видалення послуг при видаленні залу
        modelBuilder.Entity<Hall>()
            .HasMany(h => h.Services)
            .WithOne(s => s.Hall)
            .HasForeignKey(s => s.HallId)
            .OnDelete(DeleteBehavior.Cascade);

        // Захист від видалення залу за наявності прив'язаних бронювань
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Hall)
            .WithMany(h => h.Bookings)
            .HasForeignKey(b => b.HallId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}