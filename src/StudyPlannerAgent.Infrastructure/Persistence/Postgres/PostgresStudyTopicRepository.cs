using Npgsql;
using StudyPlannerAgent.Application.Abstractions;
using StudyPlannerAgent.Domain.Entities;

namespace StudyPlannerAgent.Infrastructure.Persistence.Postgres;

public sealed class PostgresStudyTopicRepository : IStudyTopicRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public PostgresStudyTopicRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<IReadOnlyCollection<StudyTopic>> GetAllAsync(CancellationToken cancellationToken)
    {
        const string sql = """
            select id, name, description
            from study_topics
            order by name;
            """;

        await using var command = _dataSource.CreateCommand(sql);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var topics = new List<StudyTopic>();

        while (await reader.ReadAsync(cancellationToken))
        {
            topics.Add(MapTopic(reader));
        }

        return topics;
    }

    public async Task<StudyTopic?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        const string sql = """
            select id, name, description
            from study_topics
            where id = @id;
            """;

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("id", id);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        return await reader.ReadAsync(cancellationToken) ? MapTopic(reader) : null;
    }

    private static StudyTopic MapTopic(NpgsqlDataReader reader)
    {
        return StudyTopic.Create(
            reader.GetGuid(0),
            reader.GetString(1),
            reader.GetString(2)).Value;
    }
}
