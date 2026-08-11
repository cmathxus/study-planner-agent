namespace StudyPlannerAgent.Application.Auth;

public sealed record AuthResponse(Guid UserId, string Name, string Email, string AccessToken);
