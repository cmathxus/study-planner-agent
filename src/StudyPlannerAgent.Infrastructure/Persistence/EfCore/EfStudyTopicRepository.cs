using Microsoft.EntityFrameworkCore;
using StudyPlannerAgent.Application.Abstractions;
using StudyPlannerAgent.Domain.Entities;

namespace StudyPlannerAgent.Infrastructure.Persistence.EfCore;

public sealed class EfStudyTopicRepository : IStudyTopicRepository
{
    private readonly StudyPlannerDbContext _dbContext;

    public EfStudyTopicRepository(StudyPlannerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<StudyTopic>> GetAllAsync(CancellationToken cancellationToken)
    {
        var topics = await _dbContext.StudyTopics
            .AsNoTracking()
            .OrderBy(topic => topic.Name)
            .ToListAsync(cancellationToken);

        return topics.Select(topic => StudyTopic.Create(topic.Id, topic.Name, topic.Description).Value).ToList();
    }

    public async Task<StudyTopic?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var topic = await _dbContext.StudyTopics
            .AsNoTracking()
            .FirstOrDefaultAsync(candidate => candidate.Id == id, cancellationToken);

        return topic is null ? null : StudyTopic.Create(topic.Id, topic.Name, topic.Description).Value;
    }
}
