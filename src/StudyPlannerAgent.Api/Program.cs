using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudyPlannerAgent.Application.Abstractions;
using StudyPlannerAgent.Application.Auth;
using StudyPlannerAgent.Application.Progress;
using StudyPlannerAgent.Application.StudyPlans;
using StudyPlannerAgent.Domain.Common;
using StudyPlannerAgent.Infrastructure.Auth;
using StudyPlannerAgent.Infrastructure.Clock;
using StudyPlannerAgent.Infrastructure.Persistence.EfCore;
using StudyPlannerAgent.Infrastructure.Persistence.InMemory;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var jwtOptions = builder.Configuration
    .GetSection(JwtOptions.SectionName)
    .Get<JwtOptions>() ?? new JwtOptions();

if (string.IsNullOrWhiteSpace(jwtOptions.Secret))
{
    if (!builder.Environment.IsDevelopment())
        throw new InvalidOperationException("Jwt:Secret is required outside Development.");

    jwtOptions = new JwtOptions
    {
        Secret = "development-only-secret-key-change-before-deploy",
        Issuer = jwtOptions.Issuer,
        Audience = jwtOptions.Audience,
        ExpirationMinutes = jwtOptions.ExpirationMinutes
    };
}

builder.Services.AddScoped<GetTodayStudyPlanUseCase>();
builder.Services.AddScoped<GetWeeklyStudyScheduleUseCase>();
builder.Services.AddScoped<RecordStudyProgressUseCase>();
builder.Services.AddScoped<GetProgressSummaryUseCase>();
builder.Services.AddScoped<RegisterUserUseCase>();
builder.Services.AddScoped<LoginUserUseCase>();

builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddSingleton(jwtOptions);
builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

var connectionString = builder.Configuration.GetConnectionString("Supabase");

if (string.IsNullOrWhiteSpace(connectionString))
{
    builder.Services.AddSingleton<InMemoryStudyData>();
    builder.Services.AddSingleton<IStudyTopicRepository, InMemoryStudyTopicRepository>();
    builder.Services.AddSingleton<IStudyScheduleRepository, InMemoryStudyScheduleRepository>();
    builder.Services.AddSingleton<IStudyProgressRepository, InMemoryStudyProgressRepository>();
    builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
}
else
{
    builder.Services.AddDbContext<StudyPlannerDbContext>(options =>
    {
        options.UseNpgsql(connectionString);
    });

    builder.Services.AddScoped<IStudyTopicRepository, EfStudyTopicRepository>();
    builder.Services.AddScoped<IStudyScheduleRepository, EfStudyScheduleRepository>();
    builder.Services.AddScoped<IStudyProgressRepository, EfStudyProgressRepository>();
    builder.Services.AddScoped<IUserRepository, EfUserRepository>();
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddOpenApi();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

var app = builder.Build();

if (!string.IsNullOrWhiteSpace(connectionString))
{
    await StudyPlannerDbInitializer.ApplyMigrationsAsync(app.Services);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();

var auth = app.MapGroup("/auth");

auth.MapPost("/register", async (
    RegisterUserRequest request,
    RegisterUserUseCase useCase,
    CancellationToken cancellationToken) =>
{
    var result = await useCase.ExecuteAsync(request, cancellationToken);

    return ToOkResult(result);
});

auth.MapPost("/login", async (
    LoginUserRequest request,
    LoginUserUseCase useCase,
    CancellationToken cancellationToken) =>
{
    var result = await useCase.ExecuteAsync(request, cancellationToken);

    return ToOkResult(result);
});

auth.MapGet("/me", (ClaimsPrincipal user) =>
{
    var userId = GetUserId(user);

    if (userId.IsFailure)
        return ToNoContentResult(userId);

    return Results.Ok(new
    {
        user_id = userId.Value,
        name = user.FindFirst(ClaimTypes.Name)?.Value,
        email = user.FindFirst(ClaimTypes.Email)?.Value
    });
}).RequireAuthorization();

var studyPlan = app.MapGroup("/study-plan").RequireAuthorization();

studyPlan.MapGet("/today", async (
    ClaimsPrincipal user,
    GetTodayStudyPlanUseCase useCase,
    CancellationToken cancellationToken) =>
{
    var userId = GetUserId(user);

    if (userId.IsFailure)
        return ToNoContentResult(userId);

    var result = await useCase.ExecuteAsync(userId.Value, cancellationToken);

    return ToOkResult(result);
});

studyPlan.MapGet("/week", async (GetWeeklyStudyScheduleUseCase useCase, CancellationToken cancellationToken) =>
{
    var result = await useCase.ExecuteAsync(cancellationToken);

    return ToOkResult(result);
});

app.MapPost("/progress", async (
    ClaimsPrincipal user,
    RecordProgressRequest request,
    RecordStudyProgressUseCase useCase,
    CancellationToken cancellationToken) =>
{
    var userId = GetUserId(user);

    if (userId.IsFailure)
        return ToNoContentResult(userId);

    var result = await useCase.ExecuteAsync(userId.Value, request, cancellationToken);

    return ToNoContentResult(result);
}).RequireAuthorization();

app.MapGet("/progress/summary", async (
    ClaimsPrincipal user,
    GetProgressSummaryUseCase useCase,
    CancellationToken cancellationToken) =>
{
    var userId = GetUserId(user);

    if (userId.IsFailure)
        return ToNoContentResult(userId);

    var result = await useCase.ExecuteAsync(userId.Value, cancellationToken);

    return ToOkResult(result);
}).RequireAuthorization();

app.Run();

static IResult ToNoContentResult(Result result)
{
    return result.IsSuccess
        ? Results.NoContent()
        : Results.BadRequest(new { result.Error.Code, result.Error.Message });
}

static IResult ToOkResult<T>(Result<T> result)
{
    return result.IsSuccess
        ? Results.Ok(result.Value)
        : Results.BadRequest(new { result.Error.Code, result.Error.Message });
}

static Result<Guid> GetUserId(ClaimsPrincipal user)
{
    var value = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    return Guid.TryParse(value, out var userId)
        ? Result<Guid>.Success(userId)
        : Result<Guid>.Failure(new Error("Auth.InvalidToken", "Authenticated user id is missing or invalid."));
}
