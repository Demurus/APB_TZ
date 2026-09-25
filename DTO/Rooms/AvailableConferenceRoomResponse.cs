namespace IlliaUlianych_APB_TZ.DTO.Rooms;

public class AvailableConferenceRoomResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int Capacity { get; set; }
    public decimal BaseHourPrice { get; set; }
    public List<int> AvailableServices { get; set; } = new();
}