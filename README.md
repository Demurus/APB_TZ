# Conference Room Booking API

ASP.NET Core Web API for managing conference rooms, searching for available rooms,
creating bookings, calculating booking prices, and generating basic business reports.

## Technologies

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- Swagger / OpenAPI
- NUnit

## Assumptions - important!

The following assumptions were made where the task specification did not explicitly define behavior:

1. Additional services are charged once per booking and not per hour.

2. Reports include bookings whose `StartTime` is inside the selected reporting period.

3. A room with existing bookings cannot be deleted because booking history should
   remain valid.

4. Two bookings that touch only at their boundaries do not conflict.

   For example:

   ```text
   10:00 - 11:00
   11:00 - 12:00
   ```

   is valid.

5. Bookings are expected to fit within one calendar day. The pricing calculator
   applies time-based modifiers for the date of the booking start.

---


## Features

### Conference Rooms

The API allows users to:

- Create a conference room
- Update conference room information
- Delete a conference room
- Assign available services to a room
- Search for available rooms by:
    - date and time
    - required capacity

### Bookings

The API supports:

- Creating room bookings
- Selecting additional services
- Checking service availability for the selected room
- Preventing overlapping bookings
- Calculating the final booking price based on booking time
- Storing the calculated booking price

### Reports

The API provides basic business analytics:

- Income report for a selected period
- Room usage report containing:
    - total number of bookings
    - total booked hours
    - total income per room

---

## Pricing Rules

The booking price is based on the room's base hourly price.
Different price modifiers are applied depending on the booking time.

| Time | Price Modifier |
|---|---|
| 06:00 - 09:00 | -10% |
| 09:00 - 12:00 | Base price |
| 12:00 - 14:00 | +15% |
| 14:00 - 18:00 | Base price |
| 18:00 - 23:00 | -20% |

Price modifiers are applied only to the part of the booking that overlaps
with the corresponding time range.

For example, if a booking spans multiple pricing periods, each part of the
booking is calculated using the appropriate modifier.

Additional selected services are then added to the final booking price.

Service prices are charged once per booking rather than per hour.

---

## API Endpoints

### Conference Rooms

#### Create Room

```text
POST /api/rooms
```

Creates a new conference room.

Example request:

```json
{
  "name": "Room D",
  "capacity": 70,
  "baseHourPrice": 2500,
  "availableServiceIds": [1, 2]
}
```

Example response:

```json
{
  "id": 4
}
```

---

#### Update Room

```text
PUT /api/rooms/{id}
```

Updates conference room information.

Example request:

```json
{
  "name": "Room D Updated",
  "capacity": 80,
  "baseHourPrice": 2800,
  "availableServiceIds": [1, 2, 3]
}
```

All fields are optional.

If a field is omitted, its current value remains unchanged.

An empty `availableServiceIds` array removes all service associations from the room.

---

#### Delete Room

```text
DELETE /api/rooms/{id}
```

Deletes a conference room.

A room that already has bookings cannot be deleted in order to preserve booking history.

In this case the API returns:

```text
409 Conflict
```

---

#### Search Available Rooms

```text
GET /api/rooms/available
```

Query parameters:

```text
startTime
endTime
capacity
```

Example:

```text
GET /api/rooms/available?startTime=2026-10-15T10:00:00&endTime=2026-10-15T12:00:00&capacity=50
```

The API returns rooms that:

- have sufficient capacity
- do not have an overlapping booking during the requested period

---

## Bookings

### Create Booking

```text
POST /api/bookings
```

Example request:

```json
{
  "roomId": 4,
  "startTime": "2026-10-15T10:00:00",
  "durationMinutes": 60,
  "selectedServiceIds": [1, 2]
}
```

Example response:

```json
{
  "id": 1,
  "totalPrice": 3300
}
```

Before creating a booking, the API checks:

- whether the room exists
- whether all selected services are available in the room
- whether the requested time conflicts with another booking

---

## Booking Conflict Rules

Two bookings conflict when their time intervals overlap.

For example:

```text
Existing booking:
10:00 - 11:00

Requested booking:
10:30 - 11:30
```

These bookings overlap, so the request is rejected with:

```text
409 Conflict
```

Adjacent bookings are allowed.

For example:

```text
10:00 - 11:00
11:00 - 12:00
```

These intervals do not overlap.

---

## Reports

### Income Report

```text
GET /api/reports/income
```

Query parameters:

```text
startDate
endDate
```

Example:

```text
GET /api/reports/income?startDate=2026-09-01T00:00:00&endDate=2026-10-01T00:00:00
```

Example response:

```json
{
  "startDate": "2026-09-01T00:00:00",
  "endDate": "2026-10-01T00:00:00",
  "totalBookings": 5,
  "totalIncome": 14800
}
```

The report includes bookings whose `StartTime` is within the requested period.

---

### Room Usage Report

```text
GET /api/reports/room-usage
```

Query parameters:

```text
startDate
endDate
```

Example response:

```json
[
  {
    "roomId": 1,
    "roomName": "Room A",
    "totalBookings": 3,
    "totalBookedHours": 4.5,
    "totalIncome": 8500
  },
  {
    "roomId": 2,
    "roomName": "Room B",
    "totalBookings": 0,
    "totalBookedHours": 0,
    "totalIncome": 0
  }
]
```

Rooms without bookings are also included in the report.

This makes it possible to identify rooms that are not being used.

---

## Validation

The API performs validation at multiple levels.

### Request Validation

ASP.NET Core validation attributes are used to validate incoming requests.

Examples include:

- room capacity must be greater than zero
- room price cannot be negative
- booking duration must be greater than zero
- IDs must be valid positive values

Invalid requests return:

```text
400 Bad Request
```

### Domain Validation

Domain entities additionally validate their own state.

For example:

- room name cannot be empty
- room capacity must be valid
- prices cannot be negative

---

## Error Handling

The application uses centralized exception handling.

Expected business situations are handled explicitly by application services.

Examples:

```text
400 Bad Request
```

Invalid input or unavailable room service.

```text
404 Not Found
```

Requested room does not exist.

```text
409 Conflict
```

Booking overlaps with an existing booking, or a room with booking history is being deleted.

```text
500 Internal Server Error
```

Unexpected server error.

Internal exception details are logged but are not exposed to API clients.

---

## Database

The application uses SQLite with Entity Framework Core.

Main database tables include:

```text
ConferenceRooms
RoomServices
Bookings
ConferenceRoomServices
BookingServices
```

---

## Swagger

Swagger / OpenAPI is enabled in Development mode.
Controller actions include XML summaries that are loaded into Swagger.

After starting the application, Swagger UI is available at:

```text
/swagger
```

For example:

```text
https://localhost:<port>/swagger
```

Swagger can be used to test all API endpoints without an external API client.

---

## Tests

The solution contains NUnit unit tests for booking price calculation.

Run the tests with:

```bash
dotnet test
```

Currently covered scenarios:

- standard hourly price
- morning discount
- peak-time surcharge
- evening discount
- booking spanning multiple pricing periods
- additional service prices

Current test result:

```text
Passed: 6
Failed: 0
Skipped: 0
Total: 6
```

---