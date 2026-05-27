namespace HotelStay.Api.Services.Providers;

/// <summary>
/// Implementation of PremierStays provider with deterministic stubbed responses.
/// Returns PascalCase formatted responses.
/// </summary>
public class PremierStaysProvider : IHotelProvider
{
    public Task<ProviderSearchResult> SearchAsync(string destination, DateTime checkIn, DateTime checkOut, string? roomType = null)
    {
        var nights = (checkOut - checkIn).Days;
        var results = new List<ProviderHotel>();

        // Deterministic responses based on destination
        if (destination.Equals("London", StringComparison.OrdinalIgnoreCase))
        {
            results.Add(new ProviderHotel
            {
                RoomType = "Standard",
                PerNightRate = 100,
                TotalPrice = 100 * nights,
                CancellationPolicy = "FreeCancellation",
                ProviderName = "PremierStays",
                Amenities = new List<string> { "WiFi", "Gym", "Business Center" },
                StarRating = 4
            });

            results.Add(new ProviderHotel
            {
                RoomType = "Deluxe",
                PerNightRate = 150,
                TotalPrice = 150 * nights,
                CancellationPolicy = "FreeCancellation",
                ProviderName = "PremierStays",
                Amenities = new List<string> { "WiFi", "Pool", "Gym", "Restaurant" },
                StarRating = 5
            });

            results.Add(new ProviderHotel
            {
                RoomType = "Suite",
                PerNightRate = 250,
                TotalPrice = 250 * nights,
                CancellationPolicy = "FreeCancellation",
                ProviderName = "PremierStays",
                Amenities = new List<string> { "WiFi", "Pool", "Gym", "Restaurant", "Spa" },
                StarRating = 5
            });
        }
        else if (destination.Equals("Paris", StringComparison.OrdinalIgnoreCase))
        {
            results.Add(new ProviderHotel
            {
                RoomType = "Standard",
                PerNightRate = 120,
                TotalPrice = 120 * nights,
                CancellationPolicy = "PaidCancellation",
                ProviderName = "PremierStays",
                Amenities = new List<string> { "WiFi", "Restaurant" },
                StarRating = 4
            });

            results.Add(new ProviderHotel
            {
                RoomType = "Deluxe",
                PerNightRate = 180,
                TotalPrice = 180 * nights,
                CancellationPolicy = "FreeCancellation",
                ProviderName = "PremierStays",
                Amenities = new List<string> { "WiFi", "Gym", "Restaurant", "Bar" },
                StarRating = 5
            });
        }

        // Filter by room type if specified
        if (!string.IsNullOrEmpty(roomType))
        {
            results = results.Where(h => h.RoomType.Equals(roomType, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        return Task.FromResult(new ProviderSearchResult { Hotels = results });
    }

    public Task<ProviderBookingResult> BookAsync(BookingDetails details)
    {
        var reference = $"PSTAYS-{details.Destination}-{DateTime.UtcNow:yyyyMMddHHmmss}";

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
            ProviderName = "PremierStays"
        });
    }
}
