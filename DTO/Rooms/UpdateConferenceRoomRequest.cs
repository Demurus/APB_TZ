using System.ComponentModel.DataAnnotations;

namespace IlliaUlianych_APB_TZ.DTO.Rooms;

public class UpdateConferenceRoomRequest
{
    public string? Name { get; set; }
    [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue)]
    public int? Capacity { get; set; }
    [System.ComponentModel.DataAnnotations.Range(0, double.MaxValue)]
    public decimal? BaseHourPrice { get; set; }
    public List<int>? AvailableServiceIds { get; set; }
}