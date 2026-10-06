using Microsoft.EntityFrameworkCore;
using ConferenceBookingApi.Data;
using ConferenceBookingApi.DTOs;
using ConferenceBookingApi.Models;
using ConferenceBookingApi.Services.Interfaces;

namespace ConferenceBookingApi.Services;

public class BookingService : IBookingService
{
    private readonly AppDbContext _context;
    private readonly IPricingService _pricingService;

    public BookingService(AppDbContext context, IPricingService pricingService)
    {
        _context = context;
        _pricingService = pricingService;
    }

    public async Task<List<HallResponseDto>> SearchAvailableHallsAsync(DateTime startTime, DateTime endTime, int capacity)
    {
        if (endTime <= startTime)
        {
            throw new ArgumentException("Час завершення має бути пізнішим за час початку.");
        }

        // Пошук залів із перетином часу броні
        var occupiedHallIds = await _context.Bookings
            .Where(b => b.StartTime < endTime && b.EndTime > startTime)
            .Select(b => b.HallId)
            .Distinct()
            .ToListAsync();

        var availableHalls = await _context.Halls
            .Include(h => h.Services)
            .Where(h => h.Capacity >= capacity && !occupiedHallIds.Contains(h.Id))
            .ToListAsync();

        return availableHalls.Select(h => new HallResponseDto
        {
            Id = h.Id,
            Name = h.Name,
            Capacity = h.Capacity,
            BasePricePerHour = h.BasePricePerHour,
            Services = h.Services.Select(s => new ServiceItemDto
            {
                Name = s.Name,
                Price = s.Price
            }).ToList()
        }).ToList();
    }

    public async Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto dto)
    {
        var endTime = dto.StartTime.AddHours(dto.DurationHours);

        var hall = await _context.Halls
            .Include(h => h.Services)
            .FirstOrDefaultAsync(h => h.Id == dto.HallId);

        if (hall == null)
        {
            throw new KeyNotFoundException($"Зал з ID {dto.HallId} не знайдено.");
        }

        // Перевірка на перетин броней у часі
        bool isAlreadyBooked = await _context.Bookings
            .AnyAsync(b => b.HallId == dto.HallId && b.StartTime < endTime && b.EndTime > dto.StartTime);

        if (isAlreadyBooked)
        {
            throw new InvalidOperationException("Обраний зал вже заброньовано на цей проміжок часу.");
        }

        decimal rentalPrice = _pricingService.CalculateRentalPrice(hall.BasePricePerHour, dto.StartTime, endTime);

        decimal servicesPrice = 0m;
        var validServiceNames = new List<string>();

        if (dto.SelectedServices.Any())
        {
            var hallServiceNames = hall.Services.ToDictionary(s => s.Name.ToLowerInvariant(), s => s);

            foreach (var requestedName in dto.SelectedServices)
            {
                var key = requestedName.Trim().ToLowerInvariant();
                if (!hallServiceNames.TryGetValue(key, out var serviceItem))
                {
                    throw new ArgumentException($"Послуга '{requestedName}' недоступна для залу '{hall.Name}'.");
                }

                servicesPrice += serviceItem.Price;
                validServiceNames.Add(serviceItem.Name);
            }
        }

        decimal totalPrice = rentalPrice + servicesPrice;

        var booking = new Booking
        {
            HallId = hall.Id,
            StartTime = dto.StartTime,
            EndTime = endTime,
            DurationHours = dto.DurationHours,
            SelectedServicesNames = string.Join(", ", validServiceNames),
            RentalPrice = rentalPrice,
            ServicesPrice = servicesPrice,
            TotalPrice = totalPrice,
            CreatedAt = DateTime.UtcNow
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return new BookingResponseDto
        {
            BookingId = booking.Id,
            HallId = hall.Id,
            HallName = hall.Name,
            StartTime = booking.StartTime,
            EndTime = booking.EndTime,
            DurationHours = booking.DurationHours,
            SelectedServices = validServiceNames,
            RentalPrice = rentalPrice,
            ServicesPrice = servicesPrice,
            TotalPrice = totalPrice,
            Message = "Бронювання успішно підтверджено!"
        };
    }
}