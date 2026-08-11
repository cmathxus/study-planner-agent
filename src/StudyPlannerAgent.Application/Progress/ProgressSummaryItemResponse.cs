namespace StudyPlannerAgent.Application.Progress;

public sealed record ProgressSummaryItemResponse(Guid TopicId, string Topic, int CurrentProgress);
