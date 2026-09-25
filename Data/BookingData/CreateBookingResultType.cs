namespace IlliaUlianych_APB_TZ.Data.BookingData;

public enum CreateBookingResultType
{
    Undefined = 0,
    Success = 1,
    RoomNotFound = 2,
    ServiceUnavailable = 3,
    TimeConflict = 4
}