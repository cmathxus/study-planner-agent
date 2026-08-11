using StudyPlannerAgent.Application.Abstractions;

namespace StudyPlannerAgent.Infrastructure.Clock;

public sealed class SystemClock : IClock
{
    public DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);
}
