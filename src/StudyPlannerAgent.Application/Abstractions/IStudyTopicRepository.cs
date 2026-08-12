using StudyPlannerAgent.Domain.Entities;

namespace StudyPlannerAgent.Application.Abstractions;

public interface IStudyTopicRepository
{
    Task<IReadOnlyCollection<StudyTopic>> GetAllAsync(CancellationToken cancellationToken);
    Task<StudyTopic?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(StudyTopic topic, CancellationToken cancellationToken);
    Task UpdateAsync(StudyTopic topic, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
