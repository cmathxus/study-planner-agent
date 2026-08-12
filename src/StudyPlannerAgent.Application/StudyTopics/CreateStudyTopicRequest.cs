namespace StudyPlannerAgent.Application.StudyTopics;

public sealed record CreateStudyTopicRequest(string Name, string Description, DayOfWeek Weekday);
