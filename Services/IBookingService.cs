using IlliaUlianych_APB_TZ.Data.BookingData;
using IlliaUlianych_APB_TZ.DTO.Rooms;

namespace IlliaUlianych_APB_TZ.Services;

public interface IBookingService
{
    Task<CreateBookingResult> BookConferenceRoom(CreateBookingRequest request);
}