using IlliaUlianych_APB_TZ.Data;
using IlliaUlianych_APB_TZ.DTO.Rooms;
using IlliaUlianych_APB_TZ.Entities;
using IlliaUlianych_APB_TZ.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IlliaUlianych_APB_TZ.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingsController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public BookingsController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
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

        var duration =
            TimeSpan.FromMinutes(request.DurationMinutes);

        var endTime =
            request.StartTime.Add(duration);

        var room = await _dbContext.ConferenceRooms
            .Include(room => room.AvailableServices)
            .FirstOrDefaultAsync(room =>
                room.Id == request.RoomId);

        if (room is null)
        {
            return NotFound(
                "Conference room was not found.");
        }

        var selectedServiceIds = request.SelectedServiceIds
            .Distinct()
            .ToList();

        var selectedServices = room.AvailableServices
            .Where(service =>
                selectedServiceIds.Contains(service.Id))
            .ToList();

        if (selectedServices.Count != selectedServiceIds.Count)
        {
            return BadRequest(
                "One or more selected services are not available for this room.");
        }

        var hasOverlap = await _dbContext.Bookings
            .AnyAsync(booking =>
                booking.RoomId == room.Id &&
                booking.StartTime < endTime &&
                booking.EndTime > request.StartTime);

        if (hasOverlap)
        {
            return Conflict(
                "Conference room is already booked for the selected time.");
        }

        var totalPrice =
            BookingPriceCalculator.CalculateFinalBookingPrice(
                room.BaseHourPrice,
                request.StartTime,
                endTime,
                selectedServices);

        var booking = new Booking(
            room,
            request.StartTime,
            duration,
            totalPrice,
            selectedServices);

        _dbContext.Bookings.Add(booking);

        await _dbContext.SaveChangesAsync();

        return StatusCode(
            StatusCodes.Status201Created,
            new CreateBookingResponse
            {
                Id = booking.Id,
                TotalPrice = booking.TotalPrice
            });
    }
}