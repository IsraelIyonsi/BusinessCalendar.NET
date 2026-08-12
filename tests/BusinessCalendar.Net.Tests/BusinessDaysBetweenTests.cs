using BusinessCalendar;

namespace BusinessCalendar.Tests;

/// <summary>
/// <see cref="BusinessCalendar.BusinessDaysBetween"/> follows the same
/// inclusive-of-both-endpoints counting semantics as Excel's NETWORKDAYS
/// function. Oracle values below are taken from Microsoft's official
/// NETWORKDAYS function documentation,
/// https://support.microsoft.com/en-us/office/networkdays-function-48e717bf-a7a3-495f-969e-5005e3eb18e7,
/// and independently reproduced by direct simulation of the documented
/// algorithm.
/// </summary>
public class BusinessDaysBetweenTests
{
    public static IEnumerable<object[]> NetworkdaysOracleCases()
    {
        var start = new DateOnly(2012, 10, 1);
        var end = new DateOnly(2013, 3, 1);

        yield return new object[] { start, end, Array.Empty<DateOnly>(), 110 };
        yield return new object[] { start, end, new[] { new DateOnly(2012, 11, 22) }, 109 };
        yield return new object[]
        {
            start,
            end,
            new[] { new DateOnly(2012, 11, 22), new DateOnly(2012, 12, 4), new DateOnly(2013, 1, 21) },
            107,
        };
    }

    [Theory]
    [MemberData(nameof(NetworkdaysOracleCases))]
    public void BusinessDaysBetween_MatchesNetworkdaysOracle(DateOnly start, DateOnly end, DateOnly[] holidays, int expected)
    {
        var calendar = new BusinessCalendar(holidays);

        var result = calendar.BusinessDaysBetween(start, end);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void BusinessDaysBetween_SameBusinessDay_ReturnsOne()
    {
        var calendar = new BusinessCalendar(Array.Empty<DateOnly>());
        var friday = new DateOnly(2026, 8, 7);

        Assert.Equal(1, calendar.BusinessDaysBetween(friday, friday));
    }

    [Fact]
    public void BusinessDaysBetween_SameWeekendDay_ReturnsZero()
    {
        var calendar = new BusinessCalendar(Array.Empty<DateOnly>());
        var saturday = new DateOnly(2026, 8, 8);

        Assert.Equal(0, calendar.BusinessDaysBetween(saturday, saturday));
    }

    [Fact]
    public void BusinessDaysBetween_FullWeek_CountsFiveWeekdays()
    {
        var calendar = new BusinessCalendar(Array.Empty<DateOnly>());
        var monday = new DateOnly(2026, 8, 10);
        var sunday = monday.AddDays(6);

        Assert.Equal(5, calendar.BusinessDaysBetween(monday, sunday));
    }

    [Fact]
    public void BusinessDaysBetween_ReversedRange_ReturnsNegativeOfForwardCount()
    {
        var calendar = new BusinessCalendar(Array.Empty<DateOnly>());
        var monday = new DateOnly(2026, 8, 10);
        var sunday = monday.AddDays(6);

        var forward = calendar.BusinessDaysBetween(monday, sunday);
        var backward = calendar.BusinessDaysBetween(sunday, monday);

        Assert.Equal(-forward, backward);
    }
}
