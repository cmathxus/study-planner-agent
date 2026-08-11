namespace StudyPlannerAgent.Infrastructure.Auth;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Secret { get; init; } = string.Empty;
    public string Issuer { get; init; } = "study-planner-agent";
    public string Audience { get; init; } = "study-planner-agent";
    public int ExpirationMinutes { get; init; } = 120;
}
