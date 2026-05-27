using HotelStay.Api.Services;

namespace HotelStay.Api.Services;

/// <summary>
/// Service for aggregating hotel search results from multiple providers.
/// Normalizes responses to a unified schema.
/// </summary>
public class HotelAggregatorService
{
    private readonly IEnumerable<IHotelProvider> _providers;

    public HotelAggregatorService(IEnumerable<IHotelProvider> providers)
    {
        _providers = providers;
    }

    /// <summary>
    /// Searches across all providers in parallel and aggregates results.
    /// </summary>
    public async Task<List<ProviderHotel>> SearchAsync(string destination, DateTime checkIn, DateTime checkOut, string? roomType = null)
    {
        var tasks = _providers.Select(p => p.SearchAsync(destination, checkIn, checkOut, roomType)).ToList();
        var results = await Task.WhenAll(tasks);

        var aggregatedHotels = results
            .SelectMany(r => r.Hotels)
            .OrderBy(h => h.PerNightRate)
            .ToList();

        return aggregatedHotels;
    }
}
