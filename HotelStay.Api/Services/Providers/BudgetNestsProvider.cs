namespace HotelStay.Api.Services.Providers;

/// <summary>
/// Implementation of BudgetNests provider with deterministic stubbed responses.
/// Returns snake_case formatted responses and filters unavailable rooms.
/// </summary>
public class BudgetNestsProvider : IHotelProvider
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
                PerNightRate = 50,
                TotalPrice = 50 * nights,
                CancellationPolicy = "PaidCancellation",
                ProviderName = "BudgetNests"
            });

            results.Add(new ProviderHotel
            {
                RoomType = "Deluxe",
                PerNightRate = 80,
                TotalPrice = 80 * nights,
                CancellationPolicy = "FreeCancellation",
                ProviderName = "BudgetNests"
            });
        }
        else if (destination.Equals("Paris", StringComparison.OrdinalIgnoreCase))
        {
            results.Add(new ProviderHotel
            {
                RoomType = "Standard",
                PerNightRate = 60,
                TotalPrice = 60 * nights,
                CancellationPolicy = "PaidCancellation",
                ProviderName = "BudgetNests"
            });

            results.Add(new ProviderHotel
            {
                RoomType = "Suite",
                PerNightRate = 150,
                TotalPrice = 150 * nights,
                CancellationPolicy = "FreeCancellation",
                ProviderName = "BudgetNests"
            });
        }
        else if (destination.Equals("Tokyo", StringComparison.OrdinalIgnoreCase))
        {
            results.Add(new ProviderHotel
            {
                RoomType = "Standard",
                PerNightRate = 70,
                TotalPrice = 70 * nights,
                CancellationPolicy = "PaidCancellation",
                ProviderName = "BudgetNests"
            });

            results.Add(new ProviderHotel
            {
                RoomType = "Deluxe",
                PerNightRate = 110,
                TotalPrice = 110 * nights,
                CancellationPolicy = "FreeCancellation",
                ProviderName = "BudgetNests"
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
        var reference = $"BDG-{DateTime.UtcNow:yyyyMMddHHmmss}";

        return Task.FromResult(new ProviderBookingResult
        {
            BookingReference = reference,
            Status = "Confirmed",
            CancellationPolicy = "PaidCancellation"
        });
    }

    public Task<BookingStatusDetails> GetBookingStatusAsync(string reference)
    {
        return Task.FromResult(new BookingStatusDetails
        {
            BookingReference = reference,
            Status = "Confirmed",
            CancellationPolicy = "PaidCancellation",
            ProviderName = "BudgetNests"
        });
    }
}
