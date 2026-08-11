using StudyPlannerAgent.Domain.Common;

namespace StudyPlannerAgent.Domain.ValueObjects;

public sealed record ProgressPercentage
{
    public const int MinimumDailyProgress = 20;

    private ProgressPercentage(int value)
    {
        Value = value;
    }

    public int Value { get; }

    public static Result<ProgressPercentage> Create(int value)
    {
        if (value < MinimumDailyProgress)
        {
            return Result<ProgressPercentage>.Failure(
                new Error("Progress.BelowMinimum", $"Daily progress must be at least {MinimumDailyProgress}%."));
        }

        if (value > 100)
        {
            return Result<ProgressPercentage>.Failure(
                new Error("Progress.AboveMaximum", "Progress cannot be greater than 100%."));
        }

        return Result<ProgressPercentage>.Success(new ProgressPercentage(value));
    }
}
