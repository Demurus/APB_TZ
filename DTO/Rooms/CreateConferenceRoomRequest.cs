using System.ComponentModel.DataAnnotations;

namespace IlliaUlianych_APB_TZ.DTO.Rooms;

public class CreateConferenceRoomRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Range(1, int.MaxValue)]
    public int Capacity { get; set; }
    
    [Range(0, double.MaxValue)]
    public decimal BaseHourPrice { get; set; }
    public List<int> AvailableServiceIds { get; set; } = new();
}