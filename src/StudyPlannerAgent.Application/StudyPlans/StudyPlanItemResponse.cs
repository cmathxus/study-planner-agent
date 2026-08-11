namespace StudyPlannerAgent.Application.StudyPlans;

public sealed record StudyPlanItemResponse(
    Guid TopicId,
    string Topic,
    string Description,
    DayOfWeek Weekday,
    int CurrentProgress);
