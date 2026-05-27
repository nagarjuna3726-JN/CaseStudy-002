namespace HotelStay.Api.Services.Providers;

/// <summary>
/// Implementation of BoutiqueCollection provider with Deluxe/Suite only.
/// Adds boutique_fee £15/night and FreeCancellation up to 72h.
/// </summary>
public class BoutiqueCollectionProvider : IHotelProvider
{
    private const decimal BoutiqueFeePerNight = 15m;

    public Task<ProviderSearchResult> SearchAsync(string destination, DateTime checkIn, DateTime checkOut, string? roomType = null)
    {
        var nights = (checkOut - checkIn).Days;
        var results = new List<ProviderHotel>();

        // Only Deluxe and Suite room types for BoutiqueCollection
        if (destination.Equals("London", StringComparison.OrdinalIgnoreCase) || 
            destination.Equals("Paris", StringComparison.OrdinalIgnoreCase))
        {
            results.Add(new ProviderHotel
            {
                RoomType = "Deluxe",
                PerNightRate = 200 + BoutiqueFeePerNight,
                TotalPrice = (200 + BoutiqueFeePerNight) * nights,
                CancellationPolicy = "FreeCancellation",
                ProviderName = "BoutiqueCollection",
                Amenities = new List<string> { "WiFi", "Gym", "Concierge", "Nightly Turndown" },
                StarRating = 5
            });

            results.Add(new ProviderHotel
            {
                RoomType = "Suite",
                PerNightRate = 350 + BoutiqueFeePerNight,
                TotalPrice = (350 + BoutiqueFeePerNight) * nights,
                CancellationPolicy = "FreeCancellation",
                ProviderName = "BoutiqueCollection",
                Amenities = new List<string> { "WiFi", "Pool", "Gym", "Spa", "Restaurant", "Concierge", "Nightly Turndown" },
                StarRating = 5
            });
        }

        // Filter by room type if specified (only Deluxe/Suite)
        if (!string.IsNullOrEmpty(roomType))
        {
            results = results.Where(h => h.RoomType.Equals(roomType, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        return Task.FromResult(new ProviderSearchResult { Hotels = results });
    }

    public Task<ProviderBookingResult> BookAsync(BookingDetails details)
    {
        var reference = $"BQTQ-{DateTime.UtcNow:yyyyMMddHHmmss}";

        return Task.FromResult(new ProviderBookingResult
        {
            BookingReference = reference,
            Status = "Confirmed",
            CancellationPolicy = "FreeCancellation"
        });
    }

    public Task<BookingStatusDetails> GetBookingStatusAsync(string reference)
    {
        return Task.FromResult(new BookingStatusDetails
        {
            BookingReference = reference,
            Status = "Confirmed",
            CancellationPolicy = "FreeCancellation",
            ProviderName = "BoutiqueCollection"
        });
    }
}
