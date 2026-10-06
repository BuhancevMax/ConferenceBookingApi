namespace ConferenceBookingApi.DTOs;

public class AnalyticsSummaryDto
{
    public decimal TotalRevenue { get; set; }
    public int TotalBookingsCount { get; set; }
    public double TotalHoursBooked { get; set; }
    public MostPopularHallDto? MostPopularHall { get; set; }
    public List<PopularServiceDto> TopServices { get; set; } = new();
}

public class MostPopularHallDto
{
    public int HallId { get; set; }
    public string HallName { get; set; } = string.Empty;
    public int BookingsCount { get; set; }
    public decimal TotalRevenue { get; set; }
}

public class PopularServiceDto
{
    public string ServiceName { get; set; } = string.Empty;
    public int OrdersCount { get; set; }
}