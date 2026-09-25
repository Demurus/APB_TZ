namespace IlliaUlianych_APB_TZ.DTO.Bookings;

public class CreateBookingRequest
{
    public int RoomId { get; set; }
    public DateTime StartTime { get; set; }
    public int DurationMinutes { get; set; }
    public List<int> SelectedServiceIds { get; set; } = new();
}