using StudyPlannerAgent.Domain.Entities;

namespace StudyPlannerAgent.Application.Abstractions;

public interface IStudyTopicRepository
{
    Task<IReadOnlyCollection<StudyTopic>> GetAllAsync(CancellationToken cancellationToken);
    Task<StudyTopic?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
