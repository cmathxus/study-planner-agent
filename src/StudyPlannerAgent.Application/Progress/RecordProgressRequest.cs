namespace StudyPlannerAgent.Application.Progress;

public sealed record RecordProgressRequest(Guid TopicId, int Percentage, string? Notes);
