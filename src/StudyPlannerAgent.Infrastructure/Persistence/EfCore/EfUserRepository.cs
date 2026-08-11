using Microsoft.EntityFrameworkCore;
using StudyPlannerAgent.Application.Abstractions;
using StudyPlannerAgent.Domain.Entities;
using StudyPlannerAgent.Infrastructure.Persistence.EfCore.Records;

namespace StudyPlannerAgent.Infrastructure.Persistence.EfCore;

public sealed class EfUserRepository : IUserRepository
{
    private readonly StudyPlannerDbContext _dbContext;

    public EfUserRepository(StudyPlannerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        _dbContext.Users.Add(new UserRecord
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            NormalizedEmail = user.NormalizedEmail,
            PasswordHash = user.PasswordHash,
            CreatedAt = user.CreatedAt
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var normalizedEmail = User.NormalizeEmail(email);

        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(candidate => candidate.NormalizedEmail == normalizedEmail, cancellationToken);

        return user is null ? null : MapUser(user);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(candidate => candidate.Id == id, cancellationToken);

        return user is null ? null : MapUser(user);
    }

    private static User MapUser(UserRecord user)
    {
        return User.Create(user.Id, user.Name, user.Email, user.PasswordHash, user.CreatedAt).Value;
    }
}
