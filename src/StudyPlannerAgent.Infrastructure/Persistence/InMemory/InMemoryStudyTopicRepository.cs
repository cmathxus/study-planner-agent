using StudyPlannerAgent.Application.Abstractions;
using StudyPlannerAgent.Domain.Entities;

namespace StudyPlannerAgent.Infrastructure.Persistence.InMemory;

public sealed class InMemoryStudyTopicRepository : IStudyTopicRepository
{
    private readonly InMemoryStudyData _data;

    public InMemoryStudyTopicRepository(InMemoryStudyData data)
    {
        _data = data;
    }

    public Task<IReadOnlyCollection<StudyTopic>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyCollection<StudyTopic>>(_data.StudyTopics);
    }

    public Task<StudyTopic?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var topic = _data.StudyTopics.FirstOrDefault(studyTopic => studyTopic.Id == id);

        return Task.FromResult(topic);
    }
}
