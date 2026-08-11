using StudyPlannerAgent.Domain.Entities;

namespace StudyPlannerAgent.Application.Abstractions;

public interface IStudyProgressRepository
{
    Task AddAsync(StudyProgressEntry progressEntry, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<StudyProgressEntry>> GetAllAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<StudyProgressEntry>> GetByTopicIdAsync(Guid topicId, CancellationToken cancellationToken);
}
