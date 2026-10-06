using ConferenceBookingApi.Models;

namespace ConferenceBookingApi.Data;

public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        // Створення схеми бази даних, якщо вона відсутня
        context.Database.EnsureCreated();

        if (context.Halls.Any())
        {
            return;
        }

        // Початкові дані згідно з ТЗ
        var halls = new List<Hall>
        {
            new Hall
            {
                Name = "Зал A",
                Capacity = 50,
                BasePricePerHour = 2000m,
                Services = new List<ServiceItem>
                {
                    new() { Name = "Проєктор", Price = 500m },
                    new() { Name = "Wi-Fi", Price = 300m }
                }
            },
            new Hall
            {
                Name = "Зал B",
                Capacity = 100,
                BasePricePerHour = 3500m,
                Services = new List<ServiceItem>
                {
                    new() { Name = "Проєктор", Price = 500m },
                    new() { Name = "Wi-Fi", Price = 300m },
                    new() { Name = "Звук", Price = 700m }
                }
            },
            new Hall
            {
                Name = "Зал C",
                Capacity = 30,
                BasePricePerHour = 1500m,
                Services = new List<ServiceItem>
                {
                    new() { Name = "Wi-Fi", Price = 300m }
                }
            }
        };

        context.Halls.AddRange(halls);
        context.SaveChanges();
    }
}