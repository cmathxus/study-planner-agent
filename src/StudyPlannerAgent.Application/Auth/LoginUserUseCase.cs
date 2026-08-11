using StudyPlannerAgent.Application.Abstractions;
using StudyPlannerAgent.Domain.Common;

namespace StudyPlannerAgent.Application.Auth;

public sealed class LoginUserUseCase
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserRepository _userRepository;

    public LoginUserUseCase(
        IJwtTokenGenerator jwtTokenGenerator,
        IPasswordHasher passwordHasher,
        IUserRepository userRepository)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _passwordHasher = passwordHasher;
        _userRepository = userRepository;
    }

    public async Task<Result<AuthResponse>> ExecuteAsync(LoginUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return Result<AuthResponse>.Failure(
                new Error("Auth.InvalidCredentials", "Email or password is invalid."));
        }

        return Result<AuthResponse>.Success(
            new AuthResponse(user.Id, user.Name, user.Email, _jwtTokenGenerator.Generate(user)));
    }
}
