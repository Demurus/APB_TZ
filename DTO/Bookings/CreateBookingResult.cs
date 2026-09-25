using IlliaUlianych_APB_TZ.Entities;

namespace IlliaUlianych_APB_TZ.DTO.Bookings;

public class CreateBookingResult
{
    public CreateBookingResultType Type { get; }
    public Booking? Booking { get; }

    private CreateBookingResult(
        CreateBookingResultType type,
        Booking? booking = null)
    {
        Type = type;
        Booking = booking;
    }

    public static CreateBookingResult Success(Booking booking)
        => new(CreateBookingResultType.Success, booking);

    public static CreateBookingResult RoomNotFound()
        => new(CreateBookingResultType.RoomNotFound);

    public static CreateBookingResult ServiceUnavailable()
        => new(CreateBookingResultType.ServiceUnavailable);
    
    public static CreateBookingResult TimeConflict()
        => new(CreateBookingResultType.TimeConflict);
}