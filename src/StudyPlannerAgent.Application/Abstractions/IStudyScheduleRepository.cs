using StudyPlannerAgent.Domain.Entities;

namespace StudyPlannerAgent.Application.Abstractions;

public interface IStudyScheduleRepository
{
    Task<IReadOnlyCollection<StudySchedule>> GetAllAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<StudySchedule>> GetByWeekdayAsync(DayOfWeek weekday, CancellationToken cancellationToken);
}
