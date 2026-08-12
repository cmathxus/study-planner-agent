using Microsoft.EntityFrameworkCore;
using StudyPlannerAgent.Application.Abstractions;
using StudyPlannerAgent.Domain.Entities;
using StudyPlannerAgent.Infrastructure.Persistence.EfCore.Records;

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

    public async Task AddAsync(StudyTopic topic, CancellationToken cancellationToken)
    {
        _dbContext.StudyTopics.Add(new StudyTopicRecord
        {
            Id = topic.Id,
            Name = topic.Name,
            Description = topic.Description
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(StudyTopic topic, CancellationToken cancellationToken)
    {
        var topicRecord = await _dbContext.StudyTopics
            .FirstOrDefaultAsync(candidate => candidate.Id == topic.Id, cancellationToken);

        if (topicRecord is null)
            return;

        topicRecord.Name = topic.Name;
        topicRecord.Description = topic.Description;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await _dbContext.StudyTopics
            .Where(topic => topic.Id == id)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
