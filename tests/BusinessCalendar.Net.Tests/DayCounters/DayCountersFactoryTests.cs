using BusinessCalendar;

namespace BusinessCalendar.Tests;

public class DayCountersFactoryTests
{
    [Theory]
    [InlineData(DayCountConvention.Actual360)]
    [InlineData(DayCountConvention.Actual365Fixed)]
    [InlineData(DayCountConvention.ActualActualIsda)]
    [InlineData(DayCountConvention.Thirty360Us)]
    [InlineData(DayCountConvention.ThirtyE360Eurobond)]
    public void Get_ReturnsNonNullDayCounterForEveryConvention(DayCountConvention convention)
    {
        var dayCounter = DayCounters.Get(convention);

        Assert.NotNull(dayCounter);
        Assert.False(string.IsNullOrWhiteSpace(dayCounter.Name));
    }

    [Fact]
    public void Get_InvalidConvention_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => DayCounters.Get((DayCountConvention)999));
    }

    [Theory]
    [InlineData(DayCountConvention.Actual360)]
    [InlineData(DayCountConvention.Actual365Fixed)]
    [InlineData(DayCountConvention.ActualActualIsda)]
    [InlineData(DayCountConvention.Thirty360Us)]
    [InlineData(DayCountConvention.ThirtyE360Eurobond)]
    public void Get_ReturnsSameSingletonInstanceOnRepeatedCalls(DayCountConvention convention)
    {
        Assert.Same(DayCounters.Get(convention), DayCounters.Get(convention));
    }

    [Fact]
    public void YearFraction_ZeroLengthPeriod_IsZeroForEveryConvention()
    {
        var date = new DateOnly(2026, 8, 7);

        foreach (DayCountConvention convention in Enum.GetValues<DayCountConvention>())
        {
            var dayCounter = DayCounters.Get(convention);
            Assert.Equal(0.0, dayCounter.YearFraction(date, date));
            Assert.Equal(0, dayCounter.DayCount(date, date));
        }
    }

    [Fact]
    public void YearFraction_ReversedDates_IsNegativeOfForwardResultForEveryConvention()
    {
        var start = new DateOnly(2026, 1, 15);
        var end = new DateOnly(2026, 8, 7);

        foreach (DayCountConvention convention in Enum.GetValues<DayCountConvention>())
        {
            var dayCounter = DayCounters.Get(convention);
            var forward = dayCounter.YearFraction(start, end);
            var backward = dayCounter.YearFraction(end, start);

            Assert.Equal(-forward, backward, precision: 12);
        }
    }
}
