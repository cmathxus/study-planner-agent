namespace StudyPlannerAgent.Infrastructure.Foundry;

public sealed class FoundryAgentOptions
{
    public const string SectionName = "Foundry";

    public string Endpoint { get; init; } = string.Empty;
    public string AgentId { get; init; } = string.Empty;
}
