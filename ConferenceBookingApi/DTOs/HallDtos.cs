using System.ComponentModel.DataAnnotations;

namespace ConferenceBookingApi.DTOs;

public class ServiceItemDto
{
    [Required(ErrorMessage = "Назва послуги обов'язкова")]
    public string Name { get; set; } = string.Empty;

    [Range(0, 1000000, ErrorMessage = "Вартість послуги не може бути від'ємною")]
    public decimal Price { get; set; }
}

public class CreateHallDto
{
    [Required(ErrorMessage = "Назва залу є обов'язковою")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Назва має бути від 2 до 100 символів")]
    public string Name { get; set; } = string.Empty;

    [Range(1, 10000, ErrorMessage = "Місткість залу має бути щонайменше 1 особа")]
    public int Capacity { get; set; }

    [Range(0.01, 1000000, ErrorMessage = "Базова вартість оренди за годину має бути більшою за 0")]
    public decimal BasePricePerHour { get; set; }

    public List<ServiceItemDto> Services { get; set; } = new();
}

public class UpdateHallDto
{
    public string? Name { get; set; }

    [Range(1, 10000, ErrorMessage = "Місткість має бути від 1 особи")]
    public int? Capacity { get; set; }

    [Range(0.01, 1000000, ErrorMessage = "Вартість має бути більшою за 0")]
    public decimal? BasePricePerHour { get; set; }

    public List<ServiceItemDto>? Services { get; set; }
}

public class HallResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public decimal BasePricePerHour { get; set; }
    public List<ServiceItemDto> Services { get; set; } = new();
}