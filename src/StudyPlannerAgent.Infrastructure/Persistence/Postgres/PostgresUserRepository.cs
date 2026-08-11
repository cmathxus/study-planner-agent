using Npgsql;
using StudyPlannerAgent.Application.Abstractions;
using StudyPlannerAgent.Domain.Entities;

namespace StudyPlannerAgent.Infrastructure.Persistence.Postgres;

public sealed class PostgresUserRepository : IUserRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public PostgresUserRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        const string sql = """
            insert into users (id, name, email, normalized_email, password_hash, created_at)
            values (@id, @name, @email, @normalized_email, @password_hash, @created_at);
            """;

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("id", user.Id);
        command.Parameters.AddWithValue("name", user.Name);
        command.Parameters.AddWithValue("email", user.Email);
        command.Parameters.AddWithValue("normalized_email", user.NormalizedEmail);
        command.Parameters.AddWithValue("password_hash", user.PasswordHash);
        command.Parameters.AddWithValue("created_at", user.CreatedAt);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        const string sql = """
            select id, name, email, normalized_email, password_hash, created_at
            from users
            where normalized_email = @normalized_email;
            """;

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("normalized_email", User.NormalizeEmail(email));

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        return await reader.ReadAsync(cancellationToken) ? MapUser(reader) : null;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        const string sql = """
            select id, name, email, normalized_email, password_hash, created_at
            from users
            where id = @id;
            """;

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("id", id);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        return await reader.ReadAsync(cancellationToken) ? MapUser(reader) : null;
    }

    private static User MapUser(NpgsqlDataReader reader)
    {
        return User.Create(
            reader.GetGuid(0),
            reader.GetString(1),
            reader.GetString(2),
            reader.GetString(4),
            reader.GetFieldValue<DateTimeOffset>(5)).Value;
    }
}
