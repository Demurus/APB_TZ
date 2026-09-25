using IlliaUlianych_APB_TZ.DTO.Reports;

namespace IlliaUlianych_APB_TZ.Services;

public interface IReportService
{
    Task<IncomeReportResponse> GetIncomeAsync(DateTime startDate, DateTime endDate);
    Task<IReadOnlyCollection<RoomUsageReportResponse>> GetRoomUsageAsync(DateTime startDate, DateTime endDate);
}