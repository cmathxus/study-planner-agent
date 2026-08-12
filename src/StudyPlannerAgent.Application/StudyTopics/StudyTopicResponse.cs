namespace StudyPlannerAgent.Application.StudyTopics;

public sealed record StudyTopicResponse(Guid Id, string Name, string Description, DayOfWeek? Weekday);
