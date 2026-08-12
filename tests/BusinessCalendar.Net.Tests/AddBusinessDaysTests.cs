using BusinessCalendar;

namespace BusinessCalendar.Tests;

/// <summary>
/// <see cref="BusinessCalendar.AddBusinessDays"/> follows the same
/// non-inclusive counting semantics as Excel's WORKDAY function: the start
/// date is never counted, regardless of whether it is itself a business day.
/// Oracle values below are taken from Microsoft's official WORKDAY function
/// documentation, https://support.microsoft.com/en-us/excel/functions/workday-function,
/// and independently reproduced by direct simulation of the documented
/// algorithm.
/// </summary>
public class AddBusinessDaysTests
{
    public static IEnumerable<object[]> WorkdayOracleCases()
    {
        var start = new DateOnly(2008, 10, 1);

        yield return new object[]
        {
            start,
            151,
            Array.Empty<DateOnly>(),
            new DateOnly(2009, 4, 30),
        };

        yield return new object[]
        {
            start,
            151,
            new[] { new DateOnly(2008, 11, 26), new DateOnly(2008, 12, 4), new DateOnly(2009, 1, 21) },
            new DateOnly(2009, 5, 5),
        };
    }

    [Theory]
    [MemberData(nameof(WorkdayOracleCases))]
    public void AddBusinessDays_MatchesWorkdayOracle(DateOnly start, int days, DateOnly[] holidays, DateOnly expected)
    {
        var calendar = new BusinessCalendar(holidays);

        var result = calendar.AddBusinessDays(start, days);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void AddBusinessDays_ZeroDays_ReturnsStartDateUnchanged()
    {
        var calendar = new BusinessCalendar(Array.Empty<DateOnly>());
        var saturday = new DateOnly(2026, 8, 8);

        Assert.Equal(saturday, calendar.AddBusinessDays(saturday, 0));
    }

    [Fact]
    public void AddBusinessDays_PositiveCount_StepsForwardSkippingWeekends()
    {
        var calendar = new BusinessCalendar(Array.Empty<DateOnly>());
        var friday = new DateOnly(2026, 8, 7);

        var result = calendar.AddBusinessDays(friday, 1);

        Assert.Equal(new DateOnly(2026, 8, 10), result);
    }

    [Fact]
    public void AddBusinessDays_NegativeCount_StepsBackwardSkippingWeekends()
    {
        var calendar = new BusinessCalendar(Array.Empty<DateOnly>());
        var monday = new DateOnly(2026, 8, 10);

        var result = calendar.AddBusinessDays(monday, -1);

        Assert.Equal(new DateOnly(2026, 8, 7), result);
    }

    [Fact]
    public void AddBusinessDays_StartOnWeekend_DoesNotCountStartDate()
    {
        var calendar = new BusinessCalendar(Array.Empty<DateOnly>());
        var saturday = new DateOnly(2026, 8, 8);

        var result = calendar.AddBusinessDays(saturday, 1);

        Assert.Equal(new DateOnly(2026, 8, 10), result);
    }

    [Fact]
    public void AddBusinessDays_SkipsHolidaysAsWellAsWeekends()
    {
        var holiday = new DateOnly(2026, 8, 11);
        var calendar = new BusinessCalendar(new[] { holiday });
        var friday = new DateOnly(2026, 8, 7);

        var result = calendar.AddBusinessDays(friday, 2);

        Assert.Equal(new DateOnly(2026, 8, 12), result);
    }
}
