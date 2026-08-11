using StudyPlannerAgent.Application.Abstractions;
using StudyPlannerAgent.Domain.Common;
using StudyPlannerAgent.Domain.Entities;

namespace StudyPlannerAgent.Application.Auth;

public sealed class RegisterUserUseCase
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserRepository _userRepository;

    public RegisterUserUseCase(
        IJwtTokenGenerator jwtTokenGenerator,
        IPasswordHasher passwordHasher,
        IUserRepository userRepository)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _passwordHasher = passwordHasher;
        _userRepository = userRepository;
    }

    public async Task<Result<AuthResponse>> ExecuteAsync(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        if (request.Password.Length < 8)
        {
            return Result<AuthResponse>.Failure(
                new Error("Auth.WeakPassword", "Password must have at least 8 characters."));
        }

        var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (existingUser is not null)
        {
            return Result<AuthResponse>.Failure(
                new Error("Auth.EmailAlreadyInUse", "Email is already in use."));
        }

        var user = User.Create(
            Guid.NewGuid(),
            request.Name,
            request.Email,
            _passwordHasher.Hash(request.Password),
            DateTimeOffset.UtcNow);

        if (user.IsFailure)
            return Result<AuthResponse>.Failure(user.Error);

        await _userRepository.AddAsync(user.Value, cancellationToken);

        return Result<AuthResponse>.Success(ToAuthResponse(user.Value));
    }

    private AuthResponse ToAuthResponse(User user)
    {
        return new AuthResponse(user.Id, user.Name, user.Email, _jwtTokenGenerator.Generate(user));
    }
}
