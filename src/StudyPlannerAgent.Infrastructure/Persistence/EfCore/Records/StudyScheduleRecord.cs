namespace StudyPlannerAgent.Infrastructure.Persistence.EfCore.Records;

public sealed class StudyScheduleRecord
{
    public Guid Id { get; set; }
    public Guid StudyTopicId { get; set; }
    public DayOfWeek Weekday { get; set; }

    public StudyTopicRecord? StudyTopic { get; set; }
}
