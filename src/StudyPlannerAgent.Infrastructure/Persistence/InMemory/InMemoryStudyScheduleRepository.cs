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

    public Task<StudySchedule?> GetByTopicIdAsync(Guid topicId, CancellationToken cancellationToken)
    {
        var schedule = _data.StudySchedules.FirstOrDefault(candidate => candidate.StudyTopicId == topicId);

        return Task.FromResult(schedule);
    }

    public Task AddAsync(StudySchedule schedule, CancellationToken cancellationToken)
    {
        _data.StudySchedules.Add(schedule);

        return Task.CompletedTask;
    }

    public Task UpdateAsync(StudySchedule schedule, CancellationToken cancellationToken)
    {
        var index = _data.StudySchedules.FindIndex(candidate => candidate.Id == schedule.Id);

        if (index >= 0)
            _data.StudySchedules[index] = schedule;

        return Task.CompletedTask;
    }

    public Task DeleteByTopicIdAsync(Guid topicId, CancellationToken cancellationToken)
    {
        _data.StudySchedules.RemoveAll(schedule => schedule.StudyTopicId == topicId);

        return Task.CompletedTask;
    }
}
