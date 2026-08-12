namespace BusinessCalendar;

/// <summary>
/// The supported day-count conventions for computing a year fraction between
/// two dates.
/// </summary>
public enum DayCountConvention
{
    /// <summary>
    /// Actual/360: actual calendar days between the two dates, divided by
    /// 360. Common in money markets and short-term lending.
    /// </summary>
    Actual360,

    /// <summary>
    /// Actual/365 Fixed: actual calendar days between the two dates, divided
    /// by 365, regardless of leap years.
    /// </summary>
    Actual365Fixed,

    /// <summary>
    /// Actual/Actual (ISDA): actual calendar days, split at each calendar
    /// year boundary and weighted by the actual length (365 or 366 days) of
    /// each calendar year the period overlaps, as defined in the 2006 ISDA
    /// Definitions section 4.16(b).
    /// </summary>
    ActualActualIsda,

    /// <summary>
    /// 30/360 US (NASD): each month is treated as having 30 days, with the
    /// month-end adjustments for the 31st and for end-of-February dates used
    /// by US bond markets, matching QuantLib's <c>Thirty360::USA</c>.
    /// </summary>
    /// <remarks>
    /// This is distinct from the ISDA 2006 Definitions section 4.16(f)
    /// "30/360, Bond Basis" convention, which has no end-of-February special
    /// case: a period from 2007-02-28 to 2007-08-31 is 183 days under pure
    /// ISDA Bond Basis but 180 days here. The February rule is also applied
    /// unconditionally rather than only when the investment is end-of-month,
    /// matching QuantLib's (and this library's verified) behavior rather
    /// than the conditional variant some sources describe.
    /// </remarks>
    Thirty360Us,

    /// <summary>
    /// 30E/360 (Eurobond Basis): each month is treated as having 30 days,
    /// with the 31st of any month rolled back to the 30th and no special
    /// treatment for February.
    /// </summary>
    ThirtyE360Eurobond,
}
