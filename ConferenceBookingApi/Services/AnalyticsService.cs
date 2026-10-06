using Microsoft.EntityFrameworkCore;
using ConferenceBookingApi.Data;
using ConferenceBookingApi.DTOs;
using ConferenceBookingApi.Services.Interfaces;

namespace ConferenceBookingApi.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly AppDbContext _context;

    public AnalyticsService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AnalyticsSummaryDto> GetSummaryReportAsync()
    {
        var bookings = await _context.Bookings.Include(b => b.Hall).ToListAsync();

        var totalRevenue = bookings.Sum(b => b.TotalPrice);
        var totalBookingsCount = bookings.Count;
        var totalHoursBooked = bookings.Sum(b => b.DurationHours);

        // Визначення найпопулярнішого залу за кількістю бронювань
        MostPopularHallDto? mostPopularHall = null;
        var topHallGroup = bookings
            .GroupBy(b => b.HallId)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault();

        if (topHallGroup != null)
        {
            var hallName = topHallGroup.First().Hall?.Name ?? $"Зал #{topHallGroup.Key}";
            mostPopularHall = new MostPopularHallDto
            {
                HallId = topHallGroup.Key,
                HallName = hallName,
                BookingsCount = topHallGroup.Count(),
                TotalRevenue = topHallGroup.Sum(b => b.TotalPrice)
            };
        }

        // Підрахунок популярності послуг
        var serviceCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var b in bookings)
        {
            if (string.IsNullOrWhiteSpace(b.SelectedServicesNames)) continue;

            var services = b.SelectedServicesNames.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            foreach (var s in services)
            {
                serviceCounts[s] = serviceCounts.GetValueOrDefault(s, 0) + 1;
            }
        }

        var topServices = serviceCounts
            .OrderByDescending(kv => kv.Value)
            .Select(kv => new PopularServiceDto
            {
                ServiceName = kv.Key,
                OrdersCount = kv.Value
            })
            .ToList();

        return new AnalyticsSummaryDto
        {
            TotalRevenue = totalRevenue,
            TotalBookingsCount = totalBookingsCount,
            TotalHoursBooked = totalHoursBooked,
            MostPopularHall = mostPopularHall,
            TopServices = topServices
        };
    }
}