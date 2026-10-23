namespace WfmApp.Domain.ValueObjects;

public sealed record ShiftInterval
{
    private ShiftInterval(DateTimeOffset start, DateTimeOffset end)
    {
        Start = start;
        End = end;
    }

    public DateTimeOffset Start { get; }
    public DateTimeOffset End { get; }
    public TimeSpan Duration => End - Start;

    public static ShiftInterval Create(DateTimeOffset start, DateTimeOffset end)
    {
        if (start == default)
            throw new ArgumentException("Start time must be set.", nameof(start));

        if (end == default)
            throw new ArgumentException("End time must be set.", nameof(end));

        if (start >= end)
            throw new ArgumentException("Start time must be before end time.", nameof(start));

        if (end - start > TimeSpan.FromHours(24))
            throw new ArgumentException("Shift duration must not exceed 24 hours.", nameof(end));

        return new ShiftInterval(start, end);
    }
}