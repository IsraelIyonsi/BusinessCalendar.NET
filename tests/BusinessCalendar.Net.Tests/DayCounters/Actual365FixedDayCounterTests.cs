using BusinessCalendar;

namespace BusinessCalendar.Tests;

public class Actual365FixedDayCounterTests
{
    private static readonly IDayCounter DayCounter = DayCounters.Actual365Fixed;

    [Theory]
    [InlineData(2026, 1, 1, 2026, 1, 1, 0)]
    [InlineData(2026, 1, 1, 2026, 2, 1, 31)]
    [InlineData(2026, 1, 1, 2027, 1, 1, 365)]
    [InlineData(2024, 1, 1, 2025, 1, 1, 366)] // leap year contributes actual 366 days
    public void DayCount_ReturnsActualCalendarDays(int y1, int m1, int d1, int y2, int m2, int d2, int expected)
    {
        var result = DayCounter.DayCount(new DateOnly(y1, m1, d1), new DateOnly(y2, m2, d2));

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(2026, 1, 1, 2027, 1, 1, 365 / 365.0)]
    [InlineData(2024, 1, 1, 2025, 1, 1, 366 / 365.0)] // denominator stays fixed at 365 even through a leap year
    public void YearFraction_DividesActualDaysByThreeHundredSixtyFiveRegardlessOfLeapYears(int y1, int m1, int d1, int y2, int m2, int d2, double expected)
    {
        var result = DayCounter.YearFraction(new DateOnly(y1, m1, d1), new DateOnly(y2, m2, d2));

        Assert.Equal(expected, result, precision: 12);
    }

    [Fact]
    public void Name_IsActual365Fixed()
    {
        Assert.Equal("Actual/365 (Fixed)", DayCounter.Name);
    }
}
