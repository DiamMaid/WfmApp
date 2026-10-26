namespace WfmApp.Domain.ValueObjects;

public sealed record ShiftBreak
{
    private ShiftBreak(DateTimeOffset start, TimeSpan duration)
    {
        Start = start;
        Duration = duration;
    }

    public DateTimeOffset Start { get; }
    public TimeSpan Duration { get; }

    public static ShiftBreak Create(DateTimeOffset start, TimeSpan duration)
    {
        if (start == default)
            throw new ArgumentException("Break start must be set.", nameof(start));

        if (duration <= TimeSpan.Zero)
            throw new ArgumentException("Break duration must be positive.", nameof(duration));

        return new ShiftBreak(start, duration);
    }
}
