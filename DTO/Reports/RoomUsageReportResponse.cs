namespace IlliaUlianych_APB_TZ.DTO.Reports;

public class RoomUsageReportResponse
{
    public int RoomId { get; set; }
    public string RoomName { get; set; } = null!;
    public int TotalBookings { get; set; }
    public double TotalBookedHours { get; set; }
    public decimal TotalIncome { get; set; }
}