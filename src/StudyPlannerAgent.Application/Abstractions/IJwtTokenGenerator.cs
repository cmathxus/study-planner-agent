using StudyPlannerAgent.Domain.Entities;

namespace StudyPlannerAgent.Application.Abstractions;

public interface IJwtTokenGenerator
{
    string Generate(User user);
}
