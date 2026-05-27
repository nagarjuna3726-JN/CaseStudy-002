using FluentValidation;
using HotelStay.Api.Models.Requests;

namespace HotelStay.Api.Validators;

/// <summary>
/// Validator for hotel booking requests.
/// Enforces document type rules: Passport for international, National ID for domestic.
/// </summary>
public class BookingRequestValidator : AbstractValidator<BookingRequest>
{
    private static readonly List<string> DomesticCities = new() { "London", "Manchester", "Edinburgh", "Bristol" };
    private static readonly List<string> InternationalCities = new() { "Paris", "Tokyo", "Sydney", "New York", "Dubai" };

    public BookingRequestValidator()
    {
        RuleFor(x => x.Destination)
            .NotEmpty().WithMessage("Destination is required.")
            .Must(IsValidDestination).WithMessage("Destination is not recognized.");

        RuleFor(x => x.HotelName)
            .NotEmpty().WithMessage("Hotel name is required.");

        RuleFor(x => x.RoomType)
            .NotEmpty().WithMessage("Room type is required.")
            .Must(IsValidRoomType).WithMessage("Room type must be Standard, Deluxe, or Suite.");

        RuleFor(x => x.CheckIn)
            .NotEmpty().WithMessage("Check-in date is required.");

        RuleFor(x => x.CheckOut)
            .NotEmpty().WithMessage("Check-out date is required.");

        RuleFor(x => x.PassengerName)
            .NotEmpty().WithMessage("Passenger name is required.");

        RuleFor(x => x.DocumentType)
            .NotEmpty().WithMessage("Document type is required.")
            .Must(x => x == "Passport" || x == "National ID").WithMessage("Document type must be Passport or National ID.");

        RuleFor(x => x.DocumentNumber)
            .NotEmpty().WithMessage("Document number is required.");

        RuleFor(x => x)
            .Must(ValidateDocumentTypeForDestination)
            .WithMessage("Passport is required for international destinations, National ID for domestic destinations.");

        RuleFor(x => x.TotalPrice)
            .GreaterThan(0).WithMessage("Total price must be greater than 0.");
    }

    private static bool IsValidDestination(string destination)
    {
        return DomesticCities.Any(c => c.Equals(destination, StringComparison.OrdinalIgnoreCase)) ||
               InternationalCities.Any(c => c.Equals(destination, StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsValidRoomType(string? roomType)
    {
        return !string.IsNullOrEmpty(roomType) &&
               (roomType.Equals("Standard", StringComparison.OrdinalIgnoreCase) ||
                roomType.Equals("Deluxe", StringComparison.OrdinalIgnoreCase) ||
                roomType.Equals("Suite", StringComparison.OrdinalIgnoreCase));
    }

    private static bool ValidateDocumentTypeForDestination(BookingRequest request)
    {
        var isInternational = InternationalCities.Any(c => c.Equals(request.Destination, StringComparison.OrdinalIgnoreCase));
        var isDomestic = DomesticCities.Any(c => c.Equals(request.Destination, StringComparison.OrdinalIgnoreCase));

        if (isInternational)
        {
            return request.DocumentType == "Passport";
        }

        if (isDomestic)
        {
            return request.DocumentType == "Passport" || request.DocumentType == "National ID";
        }

        return false;
    }
}
