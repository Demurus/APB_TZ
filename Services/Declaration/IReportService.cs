using IlliaUlianych_APB_TZ.DTO.Reports;

namespace IlliaUlianych_APB_TZ.Services;

public interface IReportService
{
    /// <summary>
    /// Calculates total booking count and income for the selected period
    /// </summary>
    Task<IncomeReportResponse> GetIncomeAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates usage statistics for every conference room in the selected period
    /// </summary>
    Task<IReadOnlyCollection<RoomUsageReportResponse>> GetRoomUsageAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);
}