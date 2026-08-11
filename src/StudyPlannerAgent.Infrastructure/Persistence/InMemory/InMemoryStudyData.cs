using StudyPlannerAgent.Domain.Entities;

namespace StudyPlannerAgent.Infrastructure.Persistence.InMemory;

public sealed class InMemoryStudyData
{
    public InMemoryStudyData()
    {
        var topics = new[]
        {
            StudyTopic.Create(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"), "C# fundamentals", "Review syntax, records, LINQ and async/await.").Value,
            StudyTopic.Create(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"), "ASP.NET Core", "Practice controllers, minimal APIs, dependency injection and middleware.").Value,
            StudyTopic.Create(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3"), "Clean Architecture", "Review domain, application, infrastructure and API boundaries.").Value,
            StudyTopic.Create(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4"), "Supabase/Postgres", "Practice tables, SQL queries and repository adapters.").Value,
            StudyTopic.Create(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa5"), "MCP and Foundry", "Study tools, MCP server transport and agent integration.").Value,
            StudyTopic.Create(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa6"), "Project practice", "Build small features and write notes about tradeoffs.").Value,
            StudyTopic.Create(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa7"), "Weekly review", "Review progress, gaps and next week's focus.").Value
        };

        StudyTopics.AddRange(topics);

        StudySchedules.AddRange(
        [
            StudySchedule.Create(Guid.NewGuid(), topics[0].Id, DayOfWeek.Monday).Value,
            StudySchedule.Create(Guid.NewGuid(), topics[1].Id, DayOfWeek.Tuesday).Value,
            StudySchedule.Create(Guid.NewGuid(), topics[2].Id, DayOfWeek.Wednesday).Value,
            StudySchedule.Create(Guid.NewGuid(), topics[3].Id, DayOfWeek.Thursday).Value,
            StudySchedule.Create(Guid.NewGuid(), topics[4].Id, DayOfWeek.Friday).Value,
            StudySchedule.Create(Guid.NewGuid(), topics[5].Id, DayOfWeek.Saturday).Value,
            StudySchedule.Create(Guid.NewGuid(), topics[6].Id, DayOfWeek.Sunday).Value
        ]);
    }

    public List<StudyTopic> StudyTopics { get; } = [];
    public List<StudySchedule> StudySchedules { get; } = [];
    public List<StudyProgressEntry> StudyProgressEntries { get; } = [];
    public List<User> Users { get; } = [];
}
