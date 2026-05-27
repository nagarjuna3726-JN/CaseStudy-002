# HotelStay.Api - Technical Specification

## Requirements Overview

Implement a hotel search and booking API for the SkyRoute Travel Platform using .NET 8 Minimal APIs.

## Functional Requirements

### 1. Hotel Search Endpoint

**Endpoint**: `GET /hotels/search`

**Query Parameters**:
- `destination` (required, string): City name for hotel search
- `checkIn` (required, string): Check-in date (format: YYYY-MM-DD)
- `checkOut` (required, string): Check-out date (format: YYYY-MM-DD)
- `roomType` (optional, string): Filter by room type (Standard, Deluxe, Suite)

**Request Validation**:
- All required parameters must be provided
- `checkOut` must be after `checkIn`
- Date format must be valid
- `roomType` must be one of the allowed values (if provided)

**Provider Integration**:

1. **PremierStays Provider**
   - Response Format: PascalCase JSON
   - Fields: RoomType, PerNightRate, TotalPrice, CancellationPolicy, ProviderName, Amenities, StarRating
   - Available Room Types: Standard, Deluxe, Suite
   - Sample Response:
     ```json
     {
       "Hotels": [
         {
           "RoomType": "Deluxe",
           "PerNightRate": 120,
           "TotalPrice": 360,
           "CancellationPolicy": "FreeCancellation",
           "ProviderName": "PremierStays",
           "Amenities": ["WiFi", "Pool", "Gym"],
           "StarRating": 5
         }
       ]
     }
     ```

2. **BudgetNests Provider**
   - Response Format: snake_case JSON
   - Fields: room_type, per_night_rate, total_price, cancellation_policy, provider_name, is_available
   - Available Room Types: Standard, Deluxe, Suite
   - **Important**: Filter out rooms where `is_available` is false
   - Sample Response:
     ```json
     {
       "hotels": [
         {
           "room_type": "standard",
           "per_night_rate": 50,
           "total_price": 150,
           "cancellation_policy": "paid_cancellation",
           "provider_name": "BudgetNests",
           "is_available": true
         }
       ]
     }
     ```

3. **BoutiqueCollection Provider** (Future - Phase 2)
   - Room Types: Deluxe, Suite only
   - Additional Fee: £15/night (boutique_fee)
   - Cancellation Policy: FreeCancellation up to 72 hours

**Response Normalization**:
- Aggregate results from all providers
- Normalize to unified schema:
  ```json
  {
    "hotels": [
      {
        "roomType": "Deluxe",
        "perNightRate": 120,
        "totalPrice": 360,
        "cancellationPolicy": "FreeCancellation",
        "providerName": "PremierStays",
        "amenities": ["WiFi", "Pool", "Gym"],
        "starRating": 5
      }
    ]
  }
  ```

**Response Status**:
- `200 OK`: Search successful with results
- `400 Bad Request`: Invalid query parameters
- `429 Too Many Requests`: Rate limit exceeded

### 2. Hotel Booking Endpoint

**Endpoint**: `POST /hotels/book`

**Request Body**:
```json
{
  "destination": "London",
  "hotelName": "Hotel Name",
  "roomType": "Deluxe",
  "checkIn": "2024-06-01",
  "checkOut": "2024-06-05",
  "passengerName": "John Doe",
  "documentType": "Passport",
  "documentNumber": "AB123456",
  "providerName": "PremierStays",
  "perNightRate": 120,
  "totalPrice": 360
}
```

**Validation Rules**:

1. **Passenger Information**:
   - `passengerName` required, non-empty string
   - `documentType` required: "Passport" or "National ID"
   - `documentNumber` required, non-empty string

2. **Document Type Rules**:
   - **International Destinations** (Paris, Tokyo, Sydney, New York, Dubai): Passport REQUIRED
   - **Domestic Destinations** (London, Manchester, Edinburgh, Bristol): National ID acceptable
   - Violations return `422 Unprocessable Entity`

3. **Destination Classification**:
   - **Domestic**: London, Manchester, Edinburgh, Bristol (minimum 2)
   - **International**: Paris, Tokyo, Sydney, New York, Dubai (minimum 3)

**Booking Process**:
1. Validate all input fields
2. Check document type against destination
3. Confirm booking with provider
4. Generate booking reference
5. Return confirmation

**Response**:
```json
{
  "bookingReference": "BK-PremierStays-20240601-001",
  "providerName": "PremierStays",
  "destination": "London",
  "hotelName": "Hotel Name",
  "roomType": "Deluxe",
  "checkIn": "2024-06-01",
  "checkOut": "2024-06-05",
  "totalPrice": 360,
  "cancellationPolicy": "FreeCancellation",
  "status": "Confirmed"
}
```

**Response Status**:
- `201 Created`: Booking successful
- `400 Bad Request`: Invalid request body
- `422 Unprocessable Entity`: Validation error (document type mismatch)
- `429 Too Many Requests`: Rate limit exceeded

### 3. Booking Status Endpoint

**Endpoint**: `GET /hotels/booking/{reference}`

**Path Parameters**:
- `reference` (required, string): Booking reference

**Response**:
```json
{
  "bookingReference": "BK-PremierStays-20240601-001",
  "status": "Confirmed",
  "providerName": "PremierStays",
  "destination": "London",
  "hotelName": "Hotel Name",
  "roomType": "Deluxe",
  "checkIn": "2024-06-01",
  "checkOut": "2024-06-05",
  "totalPrice": 360,
  "cancellationPolicy": "FreeCancellation"
}
```

**Response Status**:
- `200 OK`: Booking found
- `404 Not Found`: Booking reference not found
- `429 Too Many Requests`: Rate limit exceeded

## Non-Functional Requirements

### 1. Provider Abstraction

**Interface**: `IHotelProvider`

```csharp
public interface IHotelProvider
{
    Task<SearchResult> SearchAsync(SearchRequest request);
    Task<BookingResult> BookAsync(BookingRequest request);
    Task<BookingStatus> GetBookingStatusAsync(string reference);
}
```

**Implementations**:
- `PremierStaysProvider`: Handles PremierStays API communication
- `BudgetNestsProvider`: Handles BudgetNests API communication
- `BoutiqueCollectionProvider`: Handles BoutiqueCollection API (Phase 2)

**Deterministic Stub Responses**:
- All providers return consistent, predictable responses
- Responses vary by search parameters for testing
- No external API calls in initial implementation

### 2. Input Validation

**Client-Side**:
- HTML5 form validation in UI (future)

**Server-Side**:
- All validations enforced on server
- FluentValidation library for complex rules
- Detailed error messages in responses

### 3. Document Validation

- Server-side enforcement of document type rules
- Clear error messages indicating required document type
- Support for future additional document types

### 4. Generic Repository Pattern

**Implementation**:
```csharp
public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(object id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}

public class GenericRepository<T> : IRepository<T> where T : class
{
    // Implementation
}
```

### 5. Unit of Work Pattern

```csharp
public interface IUnitOfWork : IDisposable
{
    IRepository<Booking> Bookings { get; }
    IRepository<Hotel> Hotels { get; }
    Task<int> SaveChangesAsync();
}
```

### 6. Global Exception Handling

**Middleware**: Catches all unhandled exceptions and returns appropriate HTTP responses

**Error Response Format**:
```json
{
  "error": "Error message",
  "statusCode": 500,
  "timestamp": "2024-06-01T10:00:00Z",
  "traceId": "0HN1GBDV5VF27:00000001"
}
```

**Handled Exceptions**:
- Validation errors: 400 Bad Request
- Document type mismatch: 422 Unprocessable Entity
- Booking not found: 404 Not Found
- Server errors: 500 Internal Server Error

### 7. Rate Limiting

**Implementation**:
- Per-IP rate limiting: 100 requests per minute
- Per-user rate limiting: 50 requests per minute (requires auth)
- Returns `429 Too Many Requests` when exceeded

**Headers**:
- `X-RateLimit-Limit`: Total requests allowed
- `X-RateLimit-Remaining`: Requests remaining
- `X-RateLimit-Reset`: Unix timestamp when limit resets

### 8. Caching

**Search Results Caching**:
- Cache key: `hotel_search:{destination}:{checkIn}:{checkOut}:{roomType}`
- TTL: 5 minutes
- Invalidated on new bookings

**Booking Cache**:
- Cache key: `hotel_booking:{reference}`
- TTL: 24 hours

**Implementation**:
- In-memory cache initially
- Extensible to Redis for distributed caching

### 9. Swagger/OpenAPI Documentation

**Features**:
- All endpoints documented with descriptions
- Request/response schemas
- Example payloads
- Authorization scheme (for future auth)
- Accessible at: `/swagger/ui` and `/swagger/json`

## Testing Requirements

### Unit Tests

1. **Provider Tests**:
   - Verify deterministic response generation
   - Test provider-specific response normalization
   - Validate available room filtering for BudgetNests

2. **Aggregation Tests**:
   - Verify combining results from multiple providers
   - Test response normalization accuracy
   - Validate room type filtering

3. **Validation Tests**:
   - Test date validation logic
   - Test document type rules
   - Test destination classification
   - Test invalid input handling

4. **Repository Tests**:
   - Test CRUD operations
   - Test Unit of Work pattern
   - Test transaction handling

### Integration Tests

1. **Endpoint Tests**:
   - Test complete search flow
   - Test complete booking flow
   - Test booking retrieval
   - Test error scenarios

2. **Provider Integration**:
   - Test provider abstraction
   - Test multiple provider queries
   - Test result aggregation

### Test Coverage
- Minimum 80% code coverage
- All critical paths covered
- Edge cases included

## Documentation Requirements

1. **README.md**:
   - Architecture overview
   - Project structure
   - Getting started guide
   - Technology stack
   - Copilot usage examples

2. **spec.md**:
   - This document
   - Complete technical specifications
   - API endpoint definitions
   - Validation rules
   - Testing requirements

3. **Code Comments**:
   - Complex logic explained
   - Public API members documented
   - Business rule rationale included

## Deployment Considerations

- Docker support for containerization
- Environment configuration via appsettings.json
- Logging setup (Serilog)
- Health check endpoint: `GET /health`

## Success Criteria

- [x] All three endpoints implemented and working
- [x] Provider abstraction with at least 2 implementations
- [x] Comprehensive input validation
- [x] Document type validation per destination
- [x] Result normalization across providers
- [x] Global exception handling
- [x] Rate limiting implemented
- [x] Caching strategy in place
- [x] Swagger documentation complete
- [x] Unit test coverage ≥ 80%
- [x] README and spec documentation complete
