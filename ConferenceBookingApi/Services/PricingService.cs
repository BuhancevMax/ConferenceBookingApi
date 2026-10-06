using ConferenceBookingApi.Services.Interfaces;

namespace ConferenceBookingApi.Services;

public class PricingService : IPricingService
{
    public decimal CalculateRentalPrice(decimal basePricePerHour, DateTime startTime, DateTime endTime)
    {
        if (endTime <= startTime)
        {
            throw new ArgumentException("Час завершення має бути пізніше за час початку.");
        }

        decimal totalRentalPrice = 0m;
        decimal pricePerMinute = basePricePerHour / 60m;

        // Похвилинний розрахунок інтервалу бронювання
        DateTime currentMinute = startTime;
        while (currentMinute < endTime)
        {
            TimeSpan timeOfDay = currentMinute.TimeOfDay;
            decimal multiplier = GetHourlyMultiplier(timeOfDay);

            totalRentalPrice += pricePerMinute * multiplier;
            currentMinute = currentMinute.AddMinutes(1);
        }

        return Math.Round(totalRentalPrice, 2, MidpointRounding.AwayFromZero);
    }

    private static decimal GetHourlyMultiplier(TimeSpan time)
    {
        var start06 = new TimeSpan(6, 0, 0);
        var start09 = new TimeSpan(9, 0, 0);
        var start12 = new TimeSpan(12, 0, 0);
        var start14 = new TimeSpan(14, 0, 0);
        var start18 = new TimeSpan(18, 0, 0);
        var start23 = new TimeSpan(23, 0, 0);

        // Пікові години (12:00 - 14:00): націнка 15%
        if (time >= start12 && time < start14)
        {
            return 1.15m;
        }

        // Ранкові години (06:00 - 09:00): знижка 10%
        if (time >= start06 && time < start09)
        {
            return 0.90m;
        }

        // Вечірні години (18:00 - 23:00): знижка 20%
        if (time >= start18 && time < start23)
        {
            return 0.80m;
        }

        // Стандартні та інші години: базова ціна
        return 1.00m;
    }
}   