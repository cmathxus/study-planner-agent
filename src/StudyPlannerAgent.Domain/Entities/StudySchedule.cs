using StudyPlannerAgent.Domain.Common;

namespace StudyPlannerAgent.Domain.Entities;

public sealed class StudySchedule
{
    private StudySchedule(Guid id, Guid studyTopicId, DayOfWeek weekday)
    {
        Id = id;
        StudyTopicId = studyTopicId;
        Weekday = weekday;
    }

    public Guid Id { get; }
    public Guid StudyTopicId { get; }
    public DayOfWeek Weekday { get; }

    public static Result<StudySchedule> Create(Guid id, Guid studyTopicId, DayOfWeek weekday)
    {
        if (id == Guid.Empty)
            return Result<StudySchedule>.Failure(new Error("StudySchedule.EmptyId", "Study schedule id cannot be empty."));

        if (studyTopicId == Guid.Empty)
            return Result<StudySchedule>.Failure(new Error("StudySchedule.EmptyTopicId", "Study topic id cannot be empty."));

        return Result<StudySchedule>.Success(new StudySchedule(id, studyTopicId, weekday));
    }
}
