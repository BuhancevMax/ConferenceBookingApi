using Microsoft.EntityFrameworkCore;
using ConferenceBookingApi.Data;
using ConferenceBookingApi.Services;
using ConferenceBookingApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Реєстрація контролерів
builder.Services.AddControllers();

// Реєстрація контексту бази даних SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Налаштування Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Conference Hall Booking API",
        Version = "v1",
        Description = "REST API для управління конференц-залами, бронюваннями, динамічного розрахунку вартості та бізнес-аналітики."
    });

    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Реєстрація сервісів бізнес-логіки в DI
builder.Services.AddScoped<IPricingService, PricingService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();

var app = builder.Build();

// Автоматична ініціалізація та наповнення БД початковими даними
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbInitializer.Initialize(dbContext);
}

// Конфігурація HTTP-конвеєра
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();