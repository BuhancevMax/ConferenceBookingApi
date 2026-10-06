using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConferenceBookingApi.Data;
using ConferenceBookingApi.DTOs;
using ConferenceBookingApi.Models;
using ConferenceBookingApi.Services.Interfaces;

namespace ConferenceBookingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HallsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IBookingService _bookingService;

    public HallsController(AppDbContext context, IBookingService bookingService)
    {
        _context = context;
        _bookingService = bookingService;
    }

    // Додавання конференц-залу
    [HttpPost]
    [ProducesResponseType(typeof(HallResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateHall([FromBody] CreateHallDto dto)
    {
        var hall = new Hall
        {
            Name = dto.Name,
            Capacity = dto.Capacity,
            BasePricePerHour = dto.BasePricePerHour,
            Services = dto.Services.Select(s => new ServiceItem
            {
                Name = s.Name,
                Price = s.Price
            }).ToList()
        };

        _context.Halls.Add(hall);
        await _context.SaveChangesAsync();

        var response = new HallResponseDto
        {
            Id = hall.Id,
            Name = hall.Name,
            Capacity = hall.Capacity,
            BasePricePerHour = hall.BasePricePerHour,
            Services = hall.Services.Select(s => new ServiceItemDto { Name = s.Name, Price = s.Price }).ToList()
        };

        return CreatedAtAction(nameof(GetHallById), new { id = hall.Id }, response);
    }

    // Редагування інформації про зал
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(HallResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateHall(int id, [FromBody] UpdateHallDto dto)
    {
        var hall = await _context.Halls
            .Include(h => h.Services)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (hall == null)
        {
            return NotFound(new { message = $"Зал з ID {id} не знайдено." });
        }

        if (!string.IsNullOrWhiteSpace(dto.Name))
        {
            hall.Name = dto.Name;
        }

        if (dto.Capacity.HasValue)
        {
            hall.Capacity = dto.Capacity.Value;
        }

        if (dto.BasePricePerHour.HasValue)
        {
            hall.BasePricePerHour = dto.BasePricePerHour.Value;
        }

        if (dto.Services != null)
        {
            _context.Services.RemoveRange(hall.Services);
            hall.Services = dto.Services.Select(s => new ServiceItem
            {
                Name = s.Name,
                Price = s.Price,
                HallId = hall.Id
            }).ToList();
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Інформацію про зал успішно оновлено.",
            hall = new HallResponseDto
            {
                Id = hall.Id,
                Name = hall.Name,
                Capacity = hall.Capacity,
                BasePricePerHour = hall.BasePricePerHour,
                Services = hall.Services.Select(s => new ServiceItemDto { Name = s.Name, Price = s.Price }).ToList()
            }
        });
    }

    // Видалення конференц-залу
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteHall(int id)
    {
        var hall = await _context.Halls
            .Include(h => h.Bookings)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (hall == null)
        {
            return NotFound(new { message = $"Зал з ID {id} не знайдено." });
        }

        if (hall.Bookings.Any(b => b.EndTime > DateTime.UtcNow))
        {
            return BadRequest(new { message = "Неможливо видалити зал, оскільки на нього є активні або майбутні бронювання." });
        }

        _context.Halls.Remove(hall);
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Конференц-зал '{hall.Name}' (ID {id}) успішно видалено." });
    }

    // Пошук доступних залів
    [HttpGet("available")]
    [ProducesResponseType(typeof(List<HallResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchAvailable(
        [FromQuery] DateTime startTime, 
        [FromQuery] DateTime endTime, 
        [FromQuery] int capacity)
    {
        try
        {
            var results = await _bookingService.SearchAvailableHallsAsync(startTime, endTime, capacity);
            return Ok(results);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Отримання списку всіх залів
    [HttpGet]
    public async Task<IActionResult> GetAllHalls()
    {
        var halls = await _context.Halls
            .Include(h => h.Services)
            .Select(h => new HallResponseDto
            {
                Id = h.Id,
                Name = h.Name,
                Capacity = h.Capacity,
                BasePricePerHour = h.BasePricePerHour,
                Services = h.Services.Select(s => new ServiceItemDto { Name = s.Name, Price = s.Price }).ToList()
            })
            .ToListAsync();

        return Ok(halls);
    }

    // Отримання залу за ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetHallById(int id)
    {
        var hall = await _context.Halls
            .Include(h => h.Services)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (hall == null)
        {
            return NotFound(new { message = $"Зал з ID {id} не знайдено." });
        }

        return Ok(new HallResponseDto
        {
            Id = hall.Id,
            Name = hall.Name,
            Capacity = hall.Capacity,
            BasePricePerHour = hall.BasePricePerHour,
            Services = hall.Services.Select(s => new ServiceItemDto { Name = s.Name, Price = s.Price }).ToList()
        });
    }
}