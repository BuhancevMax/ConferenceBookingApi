using ConferenceBookingApi.DTOs;

namespace ConferenceBookingApi.Services.Interfaces;

public interface IAnalyticsService
{
    Task<AnalyticsSummaryDto> GetSummaryReportAsync();
}