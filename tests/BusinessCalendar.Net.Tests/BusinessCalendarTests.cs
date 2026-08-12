using BusinessCalendar;

namespace BusinessCalendar.Tests;

public class BusinessCalendarTests
{
    private static readonly DateOnly IndependenceDay2026 = new(2026, 7, 4);
    private static readonly DateOnly Thanksgiving2026 = new(2026, 11, 26);

    private static BusinessCalendar CreateUsCalendar() =>
        new(new[] { IndependenceDay2026, Thanksgiving2026 });

    [Fact]
    public void Constructor_NullHolidays_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new BusinessCalendar(null!));
    }

    [Fact]
    public void Constructor_DuplicateHolidays_AreDeduplicated()
    {
        var calendar = new BusinessCalendar(new[] { IndependenceDay2026, IndependenceDay2026 });

        Assert.Single(calendar.Holidays);
    }

    [Fact]
    public void Constructor_NoWeekendRule_DefaultsToSaturdaySunday()
    {
        var calendar = new BusinessCalendar(Array.Empty<DateOnly>());

        Assert.Same(WeekendRule.SaturdaySunday, calendar.WeekendRule);
    }

    [Fact]
    public void Constructor_ExplicitNullWeekendRule_DefaultsToSaturdaySunday()
    {
        var calendar = new BusinessCalendar(Array.Empty<DateOnly>(), weekendRule: null);

        Assert.Same(WeekendRule.SaturdaySunday, calendar.WeekendRule);
    }

    [Theory]
    [InlineData(2026, 8, 7, true)]   // Friday
    [InlineData(2026, 8, 8, false)]  // Saturday
    [InlineData(2026, 8, 9, false)]  // Sunday
    [InlineData(2026, 8, 10, true)]  // Monday
    public void IsWeekend_FollowsDefaultRule(int year, int month, int day, bool expectedBusinessEligible)
    {
        var calendar = CreateUsCalendar();
        var date = new DateOnly(year, month, day);

        Assert.Equal(!expectedBusinessEligible, calendar.IsWeekend(date));
    }

    [Fact]
    public void IsHoliday_TrueOnlyForSuppliedHolidays()
    {
        var calendar = CreateUsCalendar();

        Assert.True(calendar.IsHoliday(IndependenceDay2026));
        Assert.False(calendar.IsHoliday(IndependenceDay2026.AddDays(1)));
    }

    [Theory]
    [InlineData(2026, 8, 7, true)]    // Friday, not a holiday
    [InlineData(2026, 8, 8, false)]   // Saturday weekend
    [InlineData(2026, 7, 4, false)]   // holiday, and also a Saturday in 2026
    [InlineData(2026, 11, 26, false)] // holiday, Thursday
    [InlineData(2026, 11, 27, true)]  // day after holiday, Friday
    public void IsBusinessDay_CombinesWeekendAndHolidayRules(int year, int month, int day, bool expected)
    {
        var calendar = CreateUsCalendar();

        Assert.Equal(expected, calendar.IsBusinessDay(new DateOnly(year, month, day)));
    }

    [Fact]
    public void NextBusinessDay_SkipsWeekendAndHoliday()
    {
        var calendar = CreateUsCalendar();

        // Wednesday 2026-11-25 -> Thursday is Thanksgiving -> next is Friday 2026-11-27.
        var result = calendar.NextBusinessDay(new DateOnly(2026, 11, 25));

        Assert.Equal(new DateOnly(2026, 11, 27), result);
    }

    [Fact]
    public void NextBusinessDay_AlwaysMovesForwardEvenWhenStartIsBusinessDay()
    {
        var calendar = CreateUsCalendar();
        var monday = new DateOnly(2026, 8, 10);

        var result = calendar.NextBusinessDay(monday);

        Assert.Equal(new DateOnly(2026, 8, 11), result);
    }

    [Fact]
    public void NextBusinessDay_CrossesWeekendBoundary()
    {
        var calendar = CreateUsCalendar();
        var friday = new DateOnly(2026, 8, 7);

        var result = calendar.NextBusinessDay(friday);

        Assert.Equal(new DateOnly(2026, 8, 10), result);
    }

    [Fact]
    public void PreviousBusinessDay_SkipsWeekendAndHoliday()
    {
        var calendar = CreateUsCalendar();

        // Friday 2026-11-27 -> Thursday is a holiday -> previous is Wednesday 2026-11-25.
        var result = calendar.PreviousBusinessDay(new DateOnly(2026, 11, 27));

        Assert.Equal(new DateOnly(2026, 11, 25), result);
    }

    [Fact]
    public void PreviousBusinessDay_AlwaysMovesBackwardEvenWhenStartIsBusinessDay()
    {
        var calendar = CreateUsCalendar();
        var monday = new DateOnly(2026, 8, 10);

        var result = calendar.PreviousBusinessDay(monday);

        Assert.Equal(new DateOnly(2026, 8, 7), result);
    }

    [Fact]
    public void PreviousBusinessDay_CrossesWeekendBoundary()
    {
        var calendar = CreateUsCalendar();
        var monday = new DateOnly(2026, 8, 10);

        var result = calendar.PreviousBusinessDay(monday);

        Assert.Equal(new DateOnly(2026, 8, 7), result);
    }

    [Fact]
    public void FridaySaturdayWeekend_TreatsThursdayAsLastBusinessDayOfWeek()
    {
        var calendar = new BusinessCalendar(Array.Empty<DateOnly>(), WeekendRule.FridaySaturday);
        var thursday = new DateOnly(2026, 8, 6);

        Assert.True(calendar.IsBusinessDay(thursday));
        Assert.False(calendar.IsBusinessDay(thursday.AddDays(1)));
        Assert.False(calendar.IsBusinessDay(thursday.AddDays(2)));
        Assert.True(calendar.IsBusinessDay(thursday.AddDays(3)));
    }
}
