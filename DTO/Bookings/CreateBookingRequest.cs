using System.ComponentModel.DataAnnotations;

namespace IlliaUlianych_APB_TZ.DTO.Bookings;

public class CreateBookingRequest
{
    [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue)]
    public int RoomId { get; set; }
    
    public DateTime StartTime { get; set; }
    
    [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue)]
    public int DurationMinutes { get; set; }
    
    public List<int> SelectedServiceIds { get; set; } = new();
}