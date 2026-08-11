namespace StudyPlannerAgent.Infrastructure.Persistence.EfCore.Records;

public sealed class StudyTopicRecord
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public List<StudyScheduleRecord> Schedules { get; set; } = [];
    public List<StudyProgressEntryRecord> ProgressEntries { get; set; } = [];
}
