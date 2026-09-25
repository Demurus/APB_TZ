using IlliaUlianych_APB_TZ.Data;
using IlliaUlianych_APB_TZ.DTO.Reports;
using Microsoft.EntityFrameworkCore;

namespace IlliaUlianych_APB_TZ.Services;

public class ReportService : IReportService
{
    private readonly ApplicationDbContext _dbContext;

    public ReportService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IncomeReportResponse> GetIncomeAsync(DateTime startDate, DateTime endDate)
    {
        var prices = await _dbContext.Bookings
            .Where(booking =>
                booking.StartTime >= startDate &&
                booking.StartTime < endDate)
            .Select(booking => booking.TotalPrice)
            .ToListAsync();

        return new IncomeReportResponse
        {
            StartDate = startDate,
            EndDate = endDate,
            TotalBookings = prices.Count,
            TotalIncome = prices.Sum()
        };
    }

    public async Task<IReadOnlyCollection<RoomUsageReportResponse>> GetRoomUsageAsync(
        DateTime startDate,
        DateTime endDate)
    {
        var rooms = await _dbContext.ConferenceRooms
            .AsNoTracking()
            .Select(room => new
            {
                room.Id,
                room.Name
            })
            .ToListAsync();

        var bookings = await _dbContext.Bookings
            .AsNoTracking()
            .Where(booking =>
                booking.StartTime >= startDate &&
                booking.StartTime < endDate)
            .Select(booking => new
            {
                booking.RoomId,
                booking.StartTime,
                booking.EndTime,
                booking.TotalPrice
            })
            .ToListAsync();

        var bookingsByRoom = bookings
            .GroupBy(booking => booking.RoomId)
            .ToDictionary(
                group => group.Key,
                group => group.ToList());

        var result = new List<RoomUsageReportResponse>(rooms.Count);

        foreach (var room in rooms)
        {
            if (!bookingsByRoom.TryGetValue(room.Id, out var roomBookings))
            {
                result.Add(new RoomUsageReportResponse
                {
                    RoomId = room.Id,
                    RoomName = room.Name,
                    TotalBookings = 0,
                    TotalBookedHours = 0,
                    TotalIncome = 0
                });

                continue;
            }

            var totalBookedHours = roomBookings.Sum(
                booking =>
                    (booking.EndTime - booking.StartTime).TotalHours);

            var totalIncome = roomBookings.Sum(
                booking => booking.TotalPrice);

            result.Add(new RoomUsageReportResponse
            {
                RoomId = room.Id,
                RoomName = room.Name,
                TotalBookings = roomBookings.Count,
                TotalBookedHours = totalBookedHours,
                TotalIncome = totalIncome
            });
        }

        return result;
    }
}