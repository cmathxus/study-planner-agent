using Microsoft.EntityFrameworkCore;
using StudyPlannerAgent.Application.Abstractions;
using StudyPlannerAgent.Domain.Entities;

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
}
