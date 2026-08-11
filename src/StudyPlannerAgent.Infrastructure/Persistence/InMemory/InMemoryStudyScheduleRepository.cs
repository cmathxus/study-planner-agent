using StudyPlannerAgent.Application.Abstractions;
using StudyPlannerAgent.Domain.Entities;

namespace StudyPlannerAgent.Infrastructure.Persistence.InMemory;

public sealed class InMemoryStudyScheduleRepository : IStudyScheduleRepository
{
    private readonly InMemoryStudyData _data;

    public InMemoryStudyScheduleRepository(InMemoryStudyData data)
    {
        _data = data;
    }

    public Task<IReadOnlyCollection<StudySchedule>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyCollection<StudySchedule>>(_data.StudySchedules);
    }

    public Task<IReadOnlyCollection<StudySchedule>> GetByWeekdayAsync(DayOfWeek weekday, CancellationToken cancellationToken)
    {
        var schedules = _data.StudySchedules
            .Where(schedule => schedule.Weekday == weekday)
            .ToList();

        return Task.FromResult<IReadOnlyCollection<StudySchedule>>(schedules);
    }
}
