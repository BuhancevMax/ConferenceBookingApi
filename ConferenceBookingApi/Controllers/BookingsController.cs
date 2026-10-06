using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConferenceBookingApi.Data;
using ConferenceBookingApi.DTOs;
using ConferenceBookingApi.Services.Interfaces;

namespace ConferenceBookingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IBookingService _bookingService;

    public BookingsController(AppDbContext context, IBookingService bookingService)
    {
        _context = context;
        _bookingService = bookingService;
    }

    // Створення бронювання
    [HttpPost]
    [ProducesResponseType(typeof(BookingResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
    {
        try
        {
            var result = await _bookingService.CreateBookingAsync(dto);
            return CreatedAtAction(nameof(GetAllBookings), new { id = result.BookingId }, result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Список усіх бронювань
    [HttpGet]
    public async Task<IActionResult> GetAllBookings()
    {
        var bookings = await _context.Bookings
            .Include(b => b.Hall)
            .OrderByDescending(b => b.StartTime)
            .Select(b => new BookingResponseDto
            {
                BookingId = b.Id,
                HallId = b.HallId,
                HallName = b.Hall != null ? b.Hall.Name : string.Empty,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                DurationHours = b.DurationHours,
                SelectedServices = string.IsNullOrWhiteSpace(b.SelectedServicesNames)
                    ? new List<string>()
                    : b.SelectedServicesNames.Split(',', StringSplitOptions.TrimEntries).ToList(),
                RentalPrice = b.RentalPrice,
                ServicesPrice = b.ServicesPrice,
                TotalPrice = b.TotalPrice,
                Message = "Підтверджено"
            })
            .ToListAsync();

        return Ok(bookings);
    }
}