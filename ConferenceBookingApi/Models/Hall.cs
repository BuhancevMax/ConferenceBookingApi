namespace ConferenceBookingApi.Models;

public class Hall
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public decimal BasePricePerHour { get; set; }
    
    public List<ServiceItem> Services { get; set; } = new();
    public List<Booking> Bookings { get; set; } = new();
}