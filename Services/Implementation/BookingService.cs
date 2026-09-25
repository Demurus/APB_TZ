using IlliaUlianych_APB_TZ.Data;
using IlliaUlianych_APB_TZ.DTO.Bookings;
using IlliaUlianych_APB_TZ.Entities;
using Microsoft.EntityFrameworkCore;

namespace IlliaUlianych_APB_TZ.Services;

internal class BookingService : IBookingService
{
    private readonly ApplicationDbContext _dbContext;

    public BookingService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<CreateBookingResult> BookConferenceRoomAsync(
        CreateBookingRequest request,
        CancellationToken cancellationToken = default)
    {
        var duration =
            TimeSpan.FromMinutes(request.DurationMinutes);

        var endTime =
            request.StartTime.Add(duration);

        var room = await _dbContext.ConferenceRooms
            .Include(room => room.AvailableServices)
            .FirstOrDefaultAsync(room =>
                room.Id == request.RoomId,
                cancellationToken);

        if (room is null)
        {
            return CreateBookingResult.RoomNotFound();
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
            return CreateBookingResult.ServiceUnavailable();
        }

        var hasOverlap = await _dbContext.Bookings
            .AnyAsync(booking =>
                booking.RoomId == room.Id &&
                booking.StartTime < endTime &&
                booking.EndTime > request.StartTime,
                cancellationToken);

        if (hasOverlap)
        {
            return CreateBookingResult.TimeConflict();
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

        await _dbContext.SaveChangesAsync(cancellationToken);

        return CreateBookingResult.Success(booking);
    }
}