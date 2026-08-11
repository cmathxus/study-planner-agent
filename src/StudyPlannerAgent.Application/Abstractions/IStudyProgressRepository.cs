using StudyPlannerAgent.Domain.Entities;

namespace StudyPlannerAgent.Application.Abstractions;

public interface IStudyProgressRepository
{
    Task AddAsync(StudyProgressEntry progressEntry, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<StudyProgressEntry>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<StudyProgressEntry>> GetByUserIdAndTopicIdAsync(Guid userId, Guid topicId, CancellationToken cancellationToken);
}
