namespace BusinessCalendar;

/// <summary>
/// Business-day adjustment conventions for rolling a date that falls on a
/// non-business day onto a nearby business day, as defined in the 2006 ISDA
/// Definitions section 4.11.
/// </summary>
public enum BusinessDayConvention
{
    /// <summary>
    /// Rolls forward to the first following day that is a business day. A
    /// date that is already a business day is returned unchanged.
    /// </summary>
    Following,

    /// <summary>
    /// Rolls forward to the first following day that is a business day,
    /// unless that day falls in the next calendar month, in which case the
    /// date is rolled backward to the first preceding business day instead.
    /// A date that is already a business day is returned unchanged.
    /// </summary>
    ModifiedFollowing,

    /// <summary>
    /// Rolls backward to the first preceding day that is a business day. A
    /// date that is already a business day is returned unchanged.
    /// </summary>
    Preceding,

    /// <summary>
    /// Rolls backward to the first preceding day that is a business day,
    /// unless that day falls in the previous calendar month, in which case
    /// the date is rolled forward to the first following business day
    /// instead. A date that is already a business day is returned unchanged.
    /// </summary>
    ModifiedPreceding,

    /// <summary>
    /// Leaves the date unchanged regardless of whether it is a business day.
    /// </summary>
    Unadjusted,
}
