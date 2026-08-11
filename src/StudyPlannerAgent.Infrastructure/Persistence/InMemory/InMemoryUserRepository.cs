using StudyPlannerAgent.Application.Abstractions;
using StudyPlannerAgent.Domain.Entities;

namespace StudyPlannerAgent.Infrastructure.Persistence.InMemory;

public sealed class InMemoryUserRepository : IUserRepository
{
    private readonly InMemoryStudyData _data;

    public InMemoryUserRepository(InMemoryStudyData data)
    {
        _data = data;
    }

    public Task AddAsync(User user, CancellationToken cancellationToken)
    {
        _data.Users.Add(user);

        return Task.CompletedTask;
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var normalizedEmail = User.NormalizeEmail(email);
        var user = _data.Users.FirstOrDefault(candidate => candidate.NormalizedEmail == normalizedEmail);

        return Task.FromResult(user);
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = _data.Users.FirstOrDefault(candidate => candidate.Id == id);

        return Task.FromResult(user);
    }
}
