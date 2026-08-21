using BusinessCalendar;

// Regression: the calendar type must be reachable via 'using BusinessCalendar;' + simple name (namespace==class collision).
namespace ConsumerScenario;

public class ConsumerScenarioTests
{
    [Fact]
    public void CalendarIsReachableBySimpleName_AndAddsBusinessDays()
    {
        var holidays = new[] { new DateOnly(2026, 7, 4) };

        var calendar = new BusinessDayCalendar(holidays);

        Assert.False(calendar.IsBusinessDay(new DateOnly(2026, 7, 4)));

        var result = calendar.AddBusinessDays(new DateOnly(2026, 8, 7), 1);

        Assert.Equal(new DateOnly(2026, 8, 10), result);
    }
}
