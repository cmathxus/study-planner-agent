using Microsoft.EntityFrameworkCore;
using StudyPlannerAgent.Application.Abstractions;
using StudyPlannerAgent.Domain.Entities;
using StudyPlannerAgent.Infrastructure.Persistence.EfCore.Records;

namespace StudyPlannerAgent.Infrastructure.Persistence.EfCore;

public sealed class EfStudyScheduleRepository : IStudyScheduleRepository
{
    private readonly StudyPlannerDbContext _dbContext;

    public EfStudyScheduleRepository(StudyPlannerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<StudySchedule>> GetAllAsync(CancellationToken cancellationToken)
    {
        var schedules = await _dbContext.StudySchedules
            .AsNoTracking()
            .OrderBy(schedule => schedule.Weekday)
            .ToListAsync(cancellationToken);

        return schedules
            .Select(schedule => StudySchedule.Create(schedule.Id, schedule.StudyTopicId, schedule.Weekday).Value)
            .ToList();
    }

    public async Task<IReadOnlyCollection<StudySchedule>> GetByWeekdayAsync(DayOfWeek weekday, CancellationToken cancellationToken)
    {
        var schedules = await _dbContext.StudySchedules
            .AsNoTracking()
            .Where(schedule => schedule.Weekday == weekday)
            .OrderBy(schedule => schedule.Id)
            .ToListAsync(cancellationToken);

        return schedules
            .Select(schedule => StudySchedule.Create(schedule.Id, schedule.StudyTopicId, schedule.Weekday).Value)
            .ToList();
    }

    public async Task<StudySchedule?> GetByTopicIdAsync(Guid topicId, CancellationToken cancellationToken)
    {
        var schedule = await _dbContext.StudySchedules
            .AsNoTracking()
            .FirstOrDefaultAsync(candidate => candidate.StudyTopicId == topicId, cancellationToken);

        return schedule is null
            ? null
            : StudySchedule.Create(schedule.Id, schedule.StudyTopicId, schedule.Weekday).Value;
    }

    public async Task AddAsync(StudySchedule schedule, CancellationToken cancellationToken)
    {
        _dbContext.StudySchedules.Add(new StudyScheduleRecord
        {
            Id = schedule.Id,
            StudyTopicId = schedule.StudyTopicId,
            Weekday = schedule.Weekday
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(StudySchedule schedule, CancellationToken cancellationToken)
    {
        var scheduleRecord = await _dbContext.StudySchedules
            .FirstOrDefaultAsync(candidate => candidate.Id == schedule.Id, cancellationToken);

        if (scheduleRecord is null)
            return;

        scheduleRecord.StudyTopicId = schedule.StudyTopicId;
        scheduleRecord.Weekday = schedule.Weekday;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteByTopicIdAsync(Guid topicId, CancellationToken cancellationToken)
    {
        await _dbContext.StudySchedules
            .Where(schedule => schedule.StudyTopicId == topicId)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
