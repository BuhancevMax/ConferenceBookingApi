namespace ConferenceBookingApi.Services.Interfaces;

public interface IPricingService
{
    decimal CalculateRentalPrice(decimal basePricePerHour, DateTime startTime, DateTime endTime);
}