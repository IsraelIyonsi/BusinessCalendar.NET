using BusinessCalendar;

namespace BusinessCalendar.Tests;

public class Actual360DayCounterTests
{
    private static readonly IDayCounter DayCounter = DayCounters.Actual360;

    [Theory]
    [InlineData(2026, 1, 1, 2026, 1, 1, 0)]
    [InlineData(2026, 1, 1, 2026, 2, 1, 31)]
    [InlineData(2026, 1, 1, 2026, 4, 1, 90)]
    [InlineData(2024, 2, 1, 2024, 3, 1, 29)] // leap year February counts its extra day as actual days
    [InlineData(2026, 1, 1, 2027, 1, 1, 365)]
    public void DayCount_ReturnsActualCalendarDays(int y1, int m1, int d1, int y2, int m2, int d2, int expected)
    {
        var result = DayCounter.DayCount(new DateOnly(y1, m1, d1), new DateOnly(y2, m2, d2));

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(2026, 1, 1, 2026, 2, 1, 31 / 360.0)]
    [InlineData(2026, 1, 1, 2027, 1, 1, 365 / 360.0)]
    public void YearFraction_DividesActualDaysByThreeHundredSixty(int y1, int m1, int d1, int y2, int m2, int d2, double expected)
    {
        var result = DayCounter.YearFraction(new DateOnly(y1, m1, d1), new DateOnly(y2, m2, d2));

        Assert.Equal(expected, result, precision: 12);
    }

    [Fact]
    public void Name_IsActual360()
    {
        Assert.Equal("Actual/360", DayCounter.Name);
    }
}
