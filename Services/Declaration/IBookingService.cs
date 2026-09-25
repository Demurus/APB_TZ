using IlliaUlianych_APB_TZ.DTO.Bookings;

namespace IlliaUlianych_APB_TZ.Services;

public interface IBookingService
{
    Task<CreateBookingResult> BookConferenceRoomAsync(CreateBookingRequest request);
}