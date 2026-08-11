using Microsoft.EntityFrameworkCore;
using StudyPlannerAgent.Application.Abstractions;
using StudyPlannerAgent.Domain.Entities;
using StudyPlannerAgent.Infrastructure.Persistence.EfCore.Records;

namespace StudyPlannerAgent.Infrastructure.Persistence.EfCore;

public sealed class EfStudyProgressRepository : IStudyProgressRepository
{
    private readonly StudyPlannerDbContext _dbContext;

    public EfStudyProgressRepository(StudyPlannerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(StudyProgressEntry progressEntry, CancellationToken cancellationToken)
    {
        _dbContext.StudyProgressEntries.Add(new StudyProgressEntryRecord
        {
            Id = progressEntry.Id,
            UserId = progressEntry.UserId,
            StudyTopicId = progressEntry.StudyTopicId,
            StudiedOn = progressEntry.StudiedOn,
            Percentage = progressEntry.Percentage.Value,
            Notes = progressEntry.Notes,
            CreatedAt = DateTimeOffset.UtcNow
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<StudyProgressEntry>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var entries = await _dbContext.StudyProgressEntries
            .AsNoTracking()
            .Where(entry => entry.UserId == userId)
            .OrderByDescending(entry => entry.StudiedOn)
            .ThenByDescending(entry => entry.CreatedAt)
            .ToListAsync(cancellationToken);

        return entries.Select(MapProgressEntry).ToList();
    }

    public async Task<IReadOnlyCollection<StudyProgressEntry>> GetByUserIdAndTopicIdAsync(Guid userId, Guid topicId, CancellationToken cancellationToken)
    {
        var entries = await _dbContext.StudyProgressEntries
            .AsNoTracking()
            .Where(entry => entry.UserId == userId && entry.StudyTopicId == topicId)
            .OrderByDescending(entry => entry.StudiedOn)
            .ThenByDescending(entry => entry.CreatedAt)
            .ToListAsync(cancellationToken);

        return entries.Select(MapProgressEntry).ToList();
    }

    private static StudyProgressEntry MapProgressEntry(StudyProgressEntryRecord entry)
    {
        return StudyProgressEntry.Create(
            entry.Id,
            entry.UserId,
            entry.StudyTopicId,
            entry.StudiedOn,
            entry.Percentage,
            entry.Notes).Value;
    }
}
