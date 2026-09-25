namespace IlliaUlianych_APB_TZ.DTO.Reports;

public class IncomeReportResponse
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public int TotalBookings { get; set; }

    public decimal TotalIncome { get; set; }
}