using Npgsql;
using StudyPlannerAgent.Application.Abstractions;
using StudyPlannerAgent.Domain.Entities;

namespace StudyPlannerAgent.Infrastructure.Persistence.Postgres;

public sealed class PostgresStudyScheduleRepository : IStudyScheduleRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public PostgresStudyScheduleRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<IReadOnlyCollection<StudySchedule>> GetAllAsync(CancellationToken cancellationToken)
    {
        const string sql = """
            select id, study_topic_id, weekday
            from study_schedules
            order by weekday;
            """;

        await using var command = _dataSource.CreateCommand(sql);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var schedules = new List<StudySchedule>();

        while (await reader.ReadAsync(cancellationToken))
        {
            schedules.Add(MapSchedule(reader));
        }

        return schedules;
    }

    public async Task<IReadOnlyCollection<StudySchedule>> GetByWeekdayAsync(DayOfWeek weekday, CancellationToken cancellationToken)
    {
        const string sql = """
            select id, study_topic_id, weekday
            from study_schedules
            where weekday = @weekday
            order by id;
            """;

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("weekday", (int)weekday);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var schedules = new List<StudySchedule>();

        while (await reader.ReadAsync(cancellationToken))
        {
            schedules.Add(MapSchedule(reader));
        }

        return schedules;
    }

    private static StudySchedule MapSchedule(NpgsqlDataReader reader)
    {
        return StudySchedule.Create(
            reader.GetGuid(0),
            reader.GetGuid(1),
            (DayOfWeek)reader.GetInt32(2)).Value;
    }
}
