using System.ComponentModel.DataAnnotations;

namespace ConferenceBookingApi.DTOs;

public class SearchAvailableHallsDto
{
    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    [Range(1, 10000, ErrorMessage = "Необхідна місткість має бути від 1 особи")]
    public int Capacity { get; set; }
}

public class CreateBookingDto
{
    [Required]
    public int HallId { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Range(0.5, 24, ErrorMessage = "Тривалість оренди має бути від 30 хвилин до 24 годин")]
    public double DurationHours { get; set; }

    public List<string> SelectedServices { get; set; } = new();
}

public class BookingResponseDto
{
    public int BookingId { get; set; }
    public int HallId { get; set; }
    public string HallName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public double DurationHours { get; set; }
    public List<string> SelectedServices { get; set; } = new();

    public decimal RentalPrice { get; set; }
    public decimal ServicesPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string Message { get; set; } = string.Empty;
}