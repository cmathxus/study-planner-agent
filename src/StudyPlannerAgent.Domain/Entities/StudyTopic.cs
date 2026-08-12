using StudyPlannerAgent.Domain.Common;

namespace StudyPlannerAgent.Domain.Entities;

public sealed class StudyTopic
{
    private StudyTopic(Guid id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public Guid Id { get; }
    public string Name { get; }
    public string Description { get; }

    public static Result<StudyTopic> Create(Guid id, string name, string description)
    {
        if (id == Guid.Empty)
            return Result<StudyTopic>.Failure(new Error("StudyTopic.EmptyId", "Study topic id cannot be empty."));

        if (string.IsNullOrWhiteSpace(name))
            return Result<StudyTopic>.Failure(new Error("StudyTopic.EmptyName", "Study topic name cannot be empty."));

        return Result<StudyTopic>.Success(new StudyTopic(id, name.Trim(), description?.Trim() ?? string.Empty));
    }
}
