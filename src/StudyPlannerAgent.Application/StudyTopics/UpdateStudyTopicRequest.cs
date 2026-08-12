namespace StudyPlannerAgent.Application.StudyTopics;

public sealed record UpdateStudyTopicRequest(string Name, string Description, DayOfWeek Weekday);
