using StudyPlannerAgent.Domain.Common;
using StudyPlannerAgent.Domain.ValueObjects;

namespace StudyPlannerAgent.Domain.Entities;

public sealed class StudyProgressEntry
{
    private StudyProgressEntry(
        Guid id,
        Guid userId,
        Guid studyTopicId,
        DateOnly studiedOn,
        ProgressPercentage percentage,
        string? notes)
    {
        Id = id;
        UserId = userId;
        StudyTopicId = studyTopicId;
        StudiedOn = studiedOn;
        Percentage = percentage;
        Notes = notes;
    }

    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid StudyTopicId { get; }
    public DateOnly StudiedOn { get; }
    public ProgressPercentage Percentage { get; }
    public string? Notes { get; }

    public static Result<StudyProgressEntry> Create(
        Guid id,
        Guid userId,
        Guid studyTopicId,
        DateOnly studiedOn,
        int percentage,
        string? notes)
    {
        if (id == Guid.Empty)
            return Result<StudyProgressEntry>.Failure(new Error("StudyProgress.EmptyId", "Study progress id cannot be empty."));

        if (studyTopicId == Guid.Empty)
            return Result<StudyProgressEntry>.Failure(new Error("StudyProgress.EmptyTopicId", "Study topic id cannot be empty."));

        if (userId == Guid.Empty)
            return Result<StudyProgressEntry>.Failure(new Error("StudyProgress.EmptyUserId", "User id cannot be empty."));

        var progressPercentage = ProgressPercentage.Create(percentage);

        if (progressPercentage.IsFailure)
            return Result<StudyProgressEntry>.Failure(progressPercentage.Error);

        return Result<StudyProgressEntry>.Success(
            new StudyProgressEntry(id, userId, studyTopicId, studiedOn, progressPercentage.Value, notes?.Trim()));
    }
}
