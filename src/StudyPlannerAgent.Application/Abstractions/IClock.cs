namespace StudyPlannerAgent.Application.Abstractions;

public interface IClock
{
    DateOnly Today { get; }
}
