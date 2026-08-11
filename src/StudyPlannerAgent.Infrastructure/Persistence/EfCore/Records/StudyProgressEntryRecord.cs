namespace StudyPlannerAgent.Infrastructure.Persistence.EfCore.Records;

public sealed class StudyProgressEntryRecord
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid StudyTopicId { get; set; }
    public DateOnly StudiedOn { get; set; }
    public int Percentage { get; set; }
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public UserRecord? User { get; set; }
    public StudyTopicRecord? StudyTopic { get; set; }
}
