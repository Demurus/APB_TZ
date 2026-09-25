using IlliaUlianych_APB_TZ.Entities;
using IlliaUlianych_APB_TZ.Services;
using NUnit.Framework;

namespace IlliaUlianych_APB_TZ.Tests;

[TestFixture]
public class BookingPriceCalculatorTests
{
    private const decimal BasePrice = 2000m;

    [Test]
    public void Calculate_StandardHour_ReturnsBasePrice()
    {
        var start = new DateTime(2026, 9, 25, 10, 0, 0);
        var end = new DateTime(2026, 9, 25, 11, 0, 0);

        var result = BookingPriceCalculator.CalculateFinalBookingPrice(
            BasePrice,
            start,
            end,
            Array.Empty<RoomService>());

        Assert.That(result, Is.EqualTo(2000m));
    }

    [Test]
    public void Calculate_MorningHour_AppliesTenPercentDiscount()
    {
        var start = new DateTime(2026, 9, 25, 7, 0, 0);
        var end = new DateTime(2026, 9, 25, 8, 0, 0);

        var result = BookingPriceCalculator.CalculateFinalBookingPrice(
            BasePrice,
            start,
            end,
            Array.Empty<RoomService>());

        Assert.That(result, Is.EqualTo(1800m));
    }

    [Test]
    public void Calculate_PeakHour_AppliesFifteenPercentIncrease()
    {
        var start = new DateTime(2026, 9, 25, 12, 0, 0);
        var end = new DateTime(2026, 9, 25, 13, 0, 0);

        var result = BookingPriceCalculator.CalculateFinalBookingPrice(
            BasePrice,
            start,
            end,
            Array.Empty<RoomService>());

        Assert.That(result, Is.EqualTo(2300m));
    }

    [Test]
    public void Calculate_EveningHour_AppliesTwentyPercentDiscount()
    {
        var start = new DateTime(2026, 9, 25, 19, 0, 0);
        var end = new DateTime(2026, 9, 25, 20, 0, 0);

        var result = BookingPriceCalculator.CalculateFinalBookingPrice(
            BasePrice,
            start,
            end,
            Array.Empty<RoomService>());

        Assert.That(result, Is.EqualTo(1600m));
    }

    [Test]
    public void Calculate_MixedRanges_AppliesModifiersOnlyToOverlap()
    {
        var start = new DateTime(2026, 9, 25, 8, 30, 0);
        var end = new DateTime(2026, 9, 25, 13, 30, 0);

        var result = BookingPriceCalculator.CalculateFinalBookingPrice(
            BasePrice,
            start,
            end,
            Array.Empty<RoomService>());

        Assert.That(result, Is.EqualTo(10350m));
    }

    [Test]
    public void Calculate_WithServices_AddsServicePrices()
    {
        var start = new DateTime(2026, 9, 25, 10, 0, 0);
        var end = new DateTime(2026, 9, 25, 11, 0, 0);

        var services = new[]
        {
            new RoomService("Projector", 500m),
            new RoomService("Wi-Fi", 300m)
        };

        var result = BookingPriceCalculator.CalculateFinalBookingPrice(
            BasePrice,
            start,
            end,
            services);

        Assert.That(result, Is.EqualTo(2800m));
    }
}