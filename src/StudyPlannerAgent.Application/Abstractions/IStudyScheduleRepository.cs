using StudyPlannerAgent.Domain.Entities;

namespace StudyPlannerAgent.Application.Abstractions;

public interface IStudyScheduleRepository
{
    Task<IReadOnlyCollection<StudySchedule>> GetAllAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<StudySchedule>> GetByWeekdayAsync(DayOfWeek weekday, CancellationToken cancellationToken);
    Task<StudySchedule?> GetByTopicIdAsync(Guid topicId, CancellationToken cancellationToken);
    Task AddAsync(StudySchedule schedule, CancellationToken cancellationToken);
    Task UpdateAsync(StudySchedule schedule, CancellationToken cancellationToken);
    Task DeleteByTopicIdAsync(Guid topicId, CancellationToken cancellationToken);
}
