using ConferenceBookingApi.DTOs;

namespace ConferenceBookingApi.Services.Interfaces;

public interface IBookingService
{
    Task<List<HallResponseDto>> SearchAvailableHallsAsync(DateTime startTime, DateTime endTime, int capacity);
    Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto dto);
}