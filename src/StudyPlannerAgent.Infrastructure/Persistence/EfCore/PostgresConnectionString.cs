using Npgsql;

namespace StudyPlannerAgent.Infrastructure.Persistence.EfCore;

public static class PostgresConnectionString
{
    public static string Normalize(string connectionString)
    {
        if (!Uri.TryCreate(connectionString, UriKind.Absolute, out var uri))
            return connectionString;

        if (!IsPostgresUri(uri))
            return connectionString;

        var database = Uri.UnescapeDataString(uri.AbsolutePath.TrimStart('/'));

        if (string.IsNullOrWhiteSpace(database))
            throw new ArgumentException("Postgres URI must include the database name.", nameof(connectionString));

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.IsDefaultPort ? 5432 : uri.Port,
            Database = database,
            SslMode = SslMode.Require
        };

        SetCredentials(builder, uri.UserInfo);

        return builder.ConnectionString;
    }

    private static bool IsPostgresUri(Uri uri)
    {
        return uri.Scheme.Equals("postgres", StringComparison.OrdinalIgnoreCase)
            || uri.Scheme.Equals("postgresql", StringComparison.OrdinalIgnoreCase);
    }

    private static void SetCredentials(NpgsqlConnectionStringBuilder builder, string userInfo)
    {
        if (string.IsNullOrWhiteSpace(userInfo))
            return;

        var parts = userInfo.Split(':', 2);

        builder.Username = Uri.UnescapeDataString(parts[0]);

        if (parts.Length > 1)
            builder.Password = Uri.UnescapeDataString(parts[1]);
    }
}
