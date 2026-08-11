using StudyPlannerAgent.Application.Abstractions;
using StudyPlannerAgent.Domain.Entities;

namespace StudyPlannerAgent.Infrastructure.Persistence.InMemory;

public sealed class InMemoryStudyProgressRepository : IStudyProgressRepository
{
    private readonly InMemoryStudyData _data;

    public InMemoryStudyProgressRepository(InMemoryStudyData data)
    {
        _data = data;
    }

    public Task AddAsync(StudyProgressEntry progressEntry, CancellationToken cancellationToken)
    {
        _data.StudyProgressEntries.Add(progressEntry);

        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<StudyProgressEntry>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var entries = _data.StudyProgressEntries
            .Where(entry => entry.UserId == userId)
            .ToList();

        return Task.FromResult<IReadOnlyCollection<StudyProgressEntry>>(entries);
    }

    public Task<IReadOnlyCollection<StudyProgressEntry>> GetByUserIdAndTopicIdAsync(Guid userId, Guid topicId, CancellationToken cancellationToken)
    {
        var entries = _data.StudyProgressEntries
            .Where(entry => entry.UserId == userId && entry.StudyTopicId == topicId)
            .ToList();

        return Task.FromResult<IReadOnlyCollection<StudyProgressEntry>>(entries);
    }
}
