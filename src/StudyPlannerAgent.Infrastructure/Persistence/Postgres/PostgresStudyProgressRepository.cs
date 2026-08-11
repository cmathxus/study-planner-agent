using Npgsql;
using StudyPlannerAgent.Application.Abstractions;
using StudyPlannerAgent.Domain.Entities;

namespace StudyPlannerAgent.Infrastructure.Persistence.Postgres;

public sealed class PostgresStudyProgressRepository : IStudyProgressRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public PostgresStudyProgressRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task AddAsync(StudyProgressEntry progressEntry, CancellationToken cancellationToken)
    {
        const string sql = """
            insert into study_progress_entries (id, user_id, study_topic_id, studied_on, percentage, notes)
            values (@id, @user_id, @study_topic_id, @studied_on, @percentage, @notes);
            """;

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("id", progressEntry.Id);
        command.Parameters.AddWithValue("user_id", progressEntry.UserId);
        command.Parameters.AddWithValue("study_topic_id", progressEntry.StudyTopicId);
        command.Parameters.AddWithValue("studied_on", progressEntry.StudiedOn);
        command.Parameters.AddWithValue("percentage", progressEntry.Percentage.Value);
        command.Parameters.AddWithValue("notes", (object?)progressEntry.Notes ?? DBNull.Value);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<StudyProgressEntry>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        const string sql = """
            select id, user_id, study_topic_id, studied_on, percentage, notes
            from study_progress_entries
            where user_id = @user_id
            order by studied_on desc, created_at desc;
            """;

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("user_id", userId);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var entries = new List<StudyProgressEntry>();

        while (await reader.ReadAsync(cancellationToken))
        {
            entries.Add(MapProgressEntry(reader));
        }

        return entries;
    }

    public async Task<IReadOnlyCollection<StudyProgressEntry>> GetByUserIdAndTopicIdAsync(Guid userId, Guid topicId, CancellationToken cancellationToken)
    {
        const string sql = """
            select id, user_id, study_topic_id, studied_on, percentage, notes
            from study_progress_entries
            where user_id = @user_id
            and study_topic_id = @study_topic_id
            order by studied_on desc, created_at desc;
            """;

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("user_id", userId);
        command.Parameters.AddWithValue("study_topic_id", topicId);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var entries = new List<StudyProgressEntry>();

        while (await reader.ReadAsync(cancellationToken))
        {
            entries.Add(MapProgressEntry(reader));
        }

        return entries;
    }

    private static StudyProgressEntry MapProgressEntry(NpgsqlDataReader reader)
    {
        var notes = reader.IsDBNull(5) ? null : reader.GetString(5);

        return StudyProgressEntry.Create(
            reader.GetGuid(0),
            reader.GetGuid(1),
            reader.GetGuid(2),
            DateOnly.FromDateTime(reader.GetDateTime(3)),
            reader.GetInt32(4),
            notes).Value;
    }
}
