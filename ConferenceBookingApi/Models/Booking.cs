namespace ConferenceBookingApi.Models;

public class Booking
{
    public int Id { get; set; }

    public int HallId { get; set; }
    public Hall? Hall { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public double DurationHours { get; set; }

    public string SelectedServicesNames { get; set; } = string.Empty;

    public decimal RentalPrice { get; set; }
    public decimal ServicesPrice { get; set; }
    public decimal TotalPrice { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}