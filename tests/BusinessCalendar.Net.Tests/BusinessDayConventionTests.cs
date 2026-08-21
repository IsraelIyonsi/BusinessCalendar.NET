using BusinessCalendar;

namespace BusinessCalendar.Tests;

public class BusinessDayConventionTests
{
    [Theory]
    [InlineData(BusinessDayConvention.Following)]
    [InlineData(BusinessDayConvention.ModifiedFollowing)]
    [InlineData(BusinessDayConvention.Preceding)]
    [InlineData(BusinessDayConvention.ModifiedPreceding)]
    [InlineData(BusinessDayConvention.Unadjusted)]
    public void Adjust_DateAlreadyBusinessDay_ReturnsUnchanged(BusinessDayConvention convention)
    {
        var calendar = new BusinessDayCalendar(Array.Empty<DateOnly>());
        var friday = new DateOnly(2026, 8, 7);

        Assert.Equal(friday, calendar.Adjust(friday, convention));
    }

    [Fact]
    public void Adjust_Unadjusted_ReturnsWeekendDateUnchanged()
    {
        var calendar = new BusinessDayCalendar(Array.Empty<DateOnly>());
        var saturday = new DateOnly(2026, 8, 8);

        Assert.Equal(saturday, calendar.Adjust(saturday, BusinessDayConvention.Unadjusted));
    }

    [Fact]
    public void Adjust_Following_RollsWeekendForwardToMonday()
    {
        var calendar = new BusinessDayCalendar(Array.Empty<DateOnly>());
        var saturday = new DateOnly(2026, 8, 8);

        var result = calendar.Adjust(saturday, BusinessDayConvention.Following);

        Assert.Equal(new DateOnly(2026, 8, 10), result);
    }

    [Fact]
    public void Adjust_Preceding_RollsWeekendBackwardToFriday()
    {
        var calendar = new BusinessDayCalendar(Array.Empty<DateOnly>());
        var sunday = new DateOnly(2026, 8, 9);

        var result = calendar.Adjust(sunday, BusinessDayConvention.Preceding);

        Assert.Equal(new DateOnly(2026, 8, 7), result);
    }

    [Fact]
    public void Adjust_ModifiedFollowing_StaysInSameMonthWhenNotCrossingMonthEnd()
    {
        var wednesdayHoliday = new DateOnly(2026, 8, 5);
        var calendar = new BusinessDayCalendar(new[] { wednesdayHoliday });

        var following = calendar.Adjust(wednesdayHoliday, BusinessDayConvention.Following);
        var modifiedFollowing = calendar.Adjust(wednesdayHoliday, BusinessDayConvention.ModifiedFollowing);

        Assert.Equal(new DateOnly(2026, 8, 6), following);
        Assert.Equal(following, modifiedFollowing);
    }

    [Fact]
    public void Adjust_ModifiedFollowing_MonthEndSaturday_RollsBackInsteadOfCrossingIntoNextMonth()
    {
        var calendar = new BusinessDayCalendar(Array.Empty<DateOnly>());
        var monthEndSaturday = new DateOnly(2026, 10, 31);

        var following = calendar.Adjust(monthEndSaturday, BusinessDayConvention.Following);
        var modifiedFollowing = calendar.Adjust(monthEndSaturday, BusinessDayConvention.ModifiedFollowing);

        // Following would cross into November; ISDA Modified Following instead
        // rolls back to the last business day of October.
        Assert.Equal(new DateOnly(2026, 11, 2), following);
        Assert.Equal(new DateOnly(2026, 10, 30), modifiedFollowing);
    }

    [Fact]
    public void Adjust_ModifiedPreceding_StaysInSameMonthWhenNotCrossingMonthStart()
    {
        var wednesdayHoliday = new DateOnly(2026, 8, 5);
        var calendar = new BusinessDayCalendar(new[] { wednesdayHoliday });

        var preceding = calendar.Adjust(wednesdayHoliday, BusinessDayConvention.Preceding);
        var modifiedPreceding = calendar.Adjust(wednesdayHoliday, BusinessDayConvention.ModifiedPreceding);

        Assert.Equal(new DateOnly(2026, 8, 4), preceding);
        Assert.Equal(preceding, modifiedPreceding);
    }

    [Fact]
    public void Adjust_ModifiedPreceding_MonthStartSunday_RollsForwardInsteadOfCrossingIntoPreviousMonth()
    {
        var calendar = new BusinessDayCalendar(Array.Empty<DateOnly>());
        var monthStartSunday = new DateOnly(2026, 11, 1);

        var preceding = calendar.Adjust(monthStartSunday, BusinessDayConvention.Preceding);
        var modifiedPreceding = calendar.Adjust(monthStartSunday, BusinessDayConvention.ModifiedPreceding);

        // Preceding would cross into October; ISDA Modified Preceding instead
        // rolls forward to the first business day of November.
        Assert.Equal(new DateOnly(2026, 10, 30), preceding);
        Assert.Equal(new DateOnly(2026, 11, 2), modifiedPreceding);
    }

    [Fact]
    public void Adjust_InvalidConvention_Throws()
    {
        var calendar = new BusinessDayCalendar(Array.Empty<DateOnly>());
        var saturday = new DateOnly(2026, 8, 8);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => calendar.Adjust(saturday, (BusinessDayConvention)999));
    }
}
