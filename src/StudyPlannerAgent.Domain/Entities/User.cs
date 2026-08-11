using StudyPlannerAgent.Domain.Common;

namespace StudyPlannerAgent.Domain.Entities;

public sealed class User
{
    private User(Guid id, string name, string email, string normalizedEmail, string passwordHash, DateTimeOffset createdAt)
    {
        Id = id;
        Name = name;
        Email = email;
        NormalizedEmail = normalizedEmail;
        PasswordHash = passwordHash;
        CreatedAt = createdAt;
    }

    public Guid Id { get; }
    public string Name { get; }
    public string Email { get; }
    public string NormalizedEmail { get; }
    public string PasswordHash { get; }
    public DateTimeOffset CreatedAt { get; }

    public static Result<User> Create(Guid id, string name, string email, string passwordHash, DateTimeOffset createdAt)
    {
        if (id == Guid.Empty)
            return Result<User>.Failure(new Error("User.EmptyId", "User id cannot be empty."));

        if (string.IsNullOrWhiteSpace(name))
            return Result<User>.Failure(new Error("User.EmptyName", "User name cannot be empty."));

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            return Result<User>.Failure(new Error("User.InvalidEmail", "User email is invalid."));

        if (string.IsNullOrWhiteSpace(passwordHash))
            return Result<User>.Failure(new Error("User.EmptyPasswordHash", "Password hash cannot be empty."));

        var normalizedEmail = NormalizeEmail(email);

        return Result<User>.Success(new User(id, name.Trim(), email.Trim(), normalizedEmail, passwordHash, createdAt));
    }

    public static string NormalizeEmail(string email)
    {
        return email.Trim().ToUpperInvariant();
    }
}
