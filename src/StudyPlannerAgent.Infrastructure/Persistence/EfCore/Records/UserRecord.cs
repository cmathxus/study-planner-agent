namespace StudyPlannerAgent.Infrastructure.Persistence.EfCore.Records;

public sealed class UserRecord
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string NormalizedEmail { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }

    public List<StudyProgressEntryRecord> ProgressEntries { get; set; } = [];
}
