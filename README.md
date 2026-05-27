# HotelStay.Api - SkyRoute Travel Platform

A .NET 8 Minimal API project implementing hotel search and booking functionality for the SkyRoute Travel Platform.

## Architecture Overview

### Core Components

1. **Provider Abstraction** (`IHotelProvider`)
   - Abstracts multiple hotel providers (PremierStays, BudgetNests, BoutiqueCollection)
   - Each provider implements standardized search and booking operations
   - Deterministic stubbed responses for consistent testing

2. **Result Normalization**
   - Normalizes JSON responses from different providers (PascalCase vs snake_case)
   - Unified schema: `roomType`, `perNightRate`, `totalPrice`, `cancellationPolicy`, `providerName`, `amenities`, `starRating`
   - Filters unavailable rooms automatically

3. **Validation Layer**
   - Input validation: destination, checkIn, checkOut with date logic
   - Document validation: Passport for international, National ID for domestic
   - Client-side and server-side validation guards

4. **Repository & Unit of Work Pattern**
   - Generic repository for data access
   - Unit of work pattern for transactional operations
   - Abstracted data layer for future persistence

5. **Cross-Cutting Concerns**
   - Global exception handling middleware
   - Rate limiting (throttling by IP/user)
   - Response caching for search results
   - Swagger/OpenAPI documentation

### API Endpoints

#### Search Hotels
```
GET /hotels/search?destination={city}&checkIn={date}&checkOut={date}&roomType={type}
```
- Queries multiple providers in parallel
- Returns normalized results with provider details
- Optional room type filtering

#### Book Hotel
```
POST /hotels/book
```
- Validates passenger and document information
- Enforces document type rules per destination
- Returns booking reference and confirmation details
- Returns 422 for validation errors

#### Get Booking Status
```
GET /hotels/booking/{reference}
```
- Retrieves booking confirmation and status
- Provider-specific reference tracking

## Project Structure

```
HotelStay.Api/
├── Program.cs                          # Application entry and DI setup
├── Endpoints/
│   ├── HotelsEndpoints.cs              # Hotel search/booking endpoints
│   └── BookingEndpoints.cs             # Booking status endpoints
├── Models/
│   ├── Requests/                       # Request DTOs
│   ├── Responses/                      # Response DTOs
│   └── Domain/                         # Domain models
├── Services/
│   ├── IHotelProvider.cs               # Provider interface
│   ├── Providers/                      # Provider implementations
│   └── HotelAggregatorService.cs       # Aggregates multiple providers
├── Repositories/
│   ├── GenericRepository.cs            # Generic repository base
│   └── UnitOfWork.cs                   # Unit of work pattern
├── Validators/
│   ├── SearchValidator.cs              # Search parameter validation
│   └── BookingValidator.cs             # Booking validation
├── Middleware/
│   ├── ExceptionHandlingMiddleware.cs  # Global exception handling
│   └── RateLimitingMiddleware.cs       # Rate limiting
├── Cache/
│   ├── CacheService.cs                 # Caching abstraction
│   └── Configuration/                  # Cache configuration
└── Configuration/
    ├── SwaggerConfiguration.cs         # Swagger setup
    └── DependencyInjection.cs          # DI container configuration

HotelStay.Tests/
├── Unit/
│   ├── Providers/
│   │   ├── PremierStaysProviderTests.cs
│   │   ├── BudgetNestsProviderTests.cs
│   │   └── BoutiqueCollectionProviderTests.cs
│   ├── Services/
│   │   ├── HotelAggregatorServiceTests.cs
│   │   └── ValidationTests.cs
│   └── Repositories/
│       └── GenericRepositoryTests.cs
└── Integration/
    ├── HotelsEndpointTests.cs
    ├── BookingEndpointTests.cs
    └── ProviderIntegrationTests.cs
```

## Copilot Usage

### Code Generation
- Generated endpoint implementations with parameter validation
- Auto-created provider stubs with deterministic responses
- Generated repository pattern boilerplate
- Created validation logic for complex business rules

### Testing Assistance
- Created unit test templates for providers
- Generated test data fixtures
- Built test scenarios for normalization logic

### Documentation
- Generated API endpoint documentation
- Created inline code comments
- Built architecture diagrams in README

## Getting Started

### Prerequisites
- .NET 8 SDK
- Visual Studio 2022 or VS Code

### Build & Run

```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run API
cd HotelStay.Api
dotnet run

# Access Swagger UI
# Navigate to: https://localhost:7001/swagger/ui

# Run tests
cd ../HotelStay.Tests
dotnet test
```

## Future Enhancements

- [ ] Database persistence (Entity Framework Core)
- [ ] Authentication & Authorization (JWT)
- [ ] Payment processing integration
- [ ] Email notifications
- [ ] Advanced search filters (price range, ratings)
- [ ] Admin dashboard
- [ ] Analytics and reporting

## Technology Stack

- **Framework**: .NET 8
- **API Style**: Minimal APIs
- **Caching**: In-memory caching (extensible to Redis)
- **Rate Limiting**: Built-in middleware
- **Documentation**: Swagger/OpenAPI
- **Testing**: xUnit, Moq
- **Validation**: FluentValidation

## License

MIT
