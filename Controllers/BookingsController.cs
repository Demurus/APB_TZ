using IlliaUlianych_APB_TZ.Data.BookingData;
using IlliaUlianych_APB_TZ.DTO.Rooms;
using IlliaUlianych_APB_TZ.Services;
using Microsoft.AspNetCore.Mvc;

namespace IlliaUlianych_APB_TZ.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    public async Task<IActionResult> BookConferenceRoom(
        CreateBookingRequest request)
    {
        if (request.DurationMinutes <= 0)
        {
            return BadRequest(
                "Booking duration must be greater than zero.");
        }
        
        var booking = await _bookingService.BookConferenceRoom(request);

        switch (booking.Type)
        {
            case CreateBookingResultType.Success:
                if (booking.Booking == null)
                {
                    throw new InvalidOperationException(
                        "Successful booking result does not contain a booking.");
                }
                
                return StatusCode(StatusCodes.Status201Created,
                    new CreateBookingResponse
                    {
                        Id = booking.Booking.Id,
                        TotalPrice = booking.Booking.TotalPrice
                    });
            case CreateBookingResultType.TimeConflict:
                return Conflict("Conference room is already booked for the selected time.");
            case CreateBookingResultType.RoomNotFound:
                return NotFound("Conference room was not found.");
            case CreateBookingResultType.ServiceUnavailable:
                return BadRequest("One or more selected services are not available.");
            default:
                throw new InvalidOperationException();
        }
    }
}