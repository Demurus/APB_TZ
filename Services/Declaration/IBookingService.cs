using IlliaUlianych_APB_TZ.DTO.Bookings;

namespace IlliaUlianych_APB_TZ.Services;

public interface IBookingService
{
    /// <summary>
    /// Creates a booking when the room exists, requested services are available, and the time slot is free
    /// </summary>
    Task<CreateBookingResult> BookConferenceRoomAsync(
        CreateBookingRequest request,
        CancellationToken cancellationToken = default);
}