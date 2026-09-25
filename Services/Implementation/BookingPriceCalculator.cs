using IlliaUlianych_APB_TZ.Entities;

namespace IlliaUlianych_APB_TZ.Services;

public static class BookingPriceCalculator
{
    private static readonly List<BookingPriceModifierRule> _rules =
    [
        new BookingPriceModifierRule
        {
            Name = "Early Hours", StartHour = 6, EndHour = 9, DiscountPercentage = 10, IncreasePercentage = 0
        },
        new BookingPriceModifierRule
        {
            Name = "Peak Hours", StartHour = 12, EndHour = 14, DiscountPercentage = 0, IncreasePercentage = 15
        },
        new BookingPriceModifierRule
        {
            Name = "Evening Hours", StartHour = 18, EndHour = 23, DiscountPercentage = 20, IncreasePercentage = 0
        }
    ];


    public static decimal CalculateFinalBookingPrice(decimal baseRoomPrice, DateTime startTime, DateTime endTime,
        IReadOnlyCollection<RoomService> selectedServices)
    {
        
        if (endTime <= startTime)
        {
            throw new ArgumentException(
                "Booking end time must be later than start time.");
        }
        
        if (baseRoomPrice < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(baseRoomPrice));
        }
        
        var bookingDuration = (endTime - startTime).TotalHours;
        decimal finalPrice = baseRoomPrice * (decimal)bookingDuration;

        foreach (var rule in _rules)
        {
            var overlap = CalculateOverlap(startTime,
                endTime,
                rule.GetRangeStartTime(startTime),
                rule.GetRangeEndTime(startTime));

            if (overlap == TimeSpan.Zero)
            {
                continue;
            }

            decimal totalOverlapHours = (decimal)overlap.TotalHours;
            var discountPerHour = baseRoomPrice * (rule.DiscountPercentage / 100m);
            var increasePerHour = baseRoomPrice * (rule.IncreasePercentage / 100m);

            finalPrice = finalPrice - discountPerHour * totalOverlapHours + increasePerHour * totalOverlapHours;
        }

        foreach (var roomService in selectedServices)
        {
            finalPrice += roomService.Price;
        }

        return finalPrice;
    }

    private static TimeSpan CalculateOverlap(
        DateTime bookingStart,
        DateTime bookingEnd,
        DateTime rangeStart,
        DateTime rangeEnd)
    {
        var overlapStart = bookingStart > rangeStart
            ? bookingStart
            : rangeStart;

        var overlapEnd = bookingEnd < rangeEnd
            ? bookingEnd
            : rangeEnd;

        if (overlapStart >= overlapEnd)
        {
            return TimeSpan.Zero;
        }

        return overlapEnd - overlapStart;
    }
}

internal class BookingPriceModifierRule
{
    public string Name { get; set; } = string.Empty;
    public int StartHour { get; set; }
    public int EndHour { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal IncreasePercentage { get; set; }

    public DateTime GetRangeStartTime(DateTime date)
    {
        return date.Date.AddHours(StartHour);
    }

    public DateTime GetRangeEndTime(DateTime date)
    {
        return date.Date.AddHours(EndHour);
    }
}