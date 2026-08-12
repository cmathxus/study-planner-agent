using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StudyPlannerAgent.Application.Abstractions;
using StudyPlannerAgent.Application.Auth;
using StudyPlannerAgent.Application.Chat;
using StudyPlannerAgent.Application.Progress;
using StudyPlannerAgent.Application.StudyPlans;
using StudyPlannerAgent.Application.StudyTopics;
using StudyPlannerAgent.Domain.Common;
using StudyPlannerAgent.Infrastructure.Auth;
using StudyPlannerAgent.Infrastructure.Clock;
using StudyPlannerAgent.Infrastructure.Foundry;
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
builder.Services.AddScoped<SendChatMessageUseCase>();
builder.Services.AddScoped<GetStudyTopicsUseCase>();
builder.Services.AddScoped<GetStudyTopicByIdUseCase>();
builder.Services.AddScoped<CreateStudyTopicUseCase>();
builder.Services.AddScoped<UpdateStudyTopicUseCase>();
builder.Services.AddScoped<DeleteStudyTopicUseCase>();
builder.Services.AddScoped<RecordStudyProgressUseCase>();
builder.Services.AddScoped<GetProgressSummaryUseCase>();
builder.Services.AddScoped<RegisterUserUseCase>();
builder.Services.AddScoped<LoginUserUseCase>();

builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddSingleton(jwtOptions);
builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

var foundryOptions = builder.Configuration
    .GetSection(FoundryAgentOptions.SectionName)
    .Get<FoundryAgentOptions>() ?? new FoundryAgentOptions();

builder.Services.AddSingleton(foundryOptions);

if (string.IsNullOrWhiteSpace(foundryOptions.Endpoint) || string.IsNullOrWhiteSpace(foundryOptions.AgentId))
    builder.Services.AddSingleton<IChatAgentClient, DisabledChatAgentClient>();
else
    builder.Services.AddSingleton<IChatAgentClient, FoundryChatAgentClient>();

var connectionString = builder.Configuration.GetConnectionString("Supabase");
var normalizedConnectionString = string.IsNullOrWhiteSpace(connectionString)
    ? null
    : PostgresConnectionString.Normalize(connectionString);

if (string.IsNullOrWhiteSpace(normalizedConnectionString))
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
        options.UseNpgsql(normalizedConnectionString);
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
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Study Planner Agent API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Informe apenas o token JWT retornado pelo login ou register."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            []
        }
    });
});
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

if (!string.IsNullOrWhiteSpace(normalizedConnectionString))
{
    await StudyPlannerDbInitializer.ApplyMigrationsAsync(app.Services);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
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

app.MapPost("/chat", async (
    ClaimsPrincipal user,
    ChatRequest request,
    SendChatMessageUseCase useCase,
    CancellationToken cancellationToken) =>
{
    var userId = GetUserId(user);

    if (userId.IsFailure)
        return ToNoContentResult(userId);

    var result = await useCase.ExecuteAsync(userId.Value, request, cancellationToken);

    return ToOkResult(result);
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

var studyTopics = app.MapGroup("/study-topics").RequireAuthorization();

studyTopics.MapGet("/", async (GetStudyTopicsUseCase useCase, CancellationToken cancellationToken) =>
{
    var result = await useCase.ExecuteAsync(cancellationToken);

    return ToOkResult(result);
});

studyTopics.MapGet("/{id:guid}", async (
    Guid id,
    GetStudyTopicByIdUseCase useCase,
    CancellationToken cancellationToken) =>
{
    var result = await useCase.ExecuteAsync(id, cancellationToken);

    return ToOkResult(result);
});

studyTopics.MapPost("/", async (
    CreateStudyTopicRequest request,
    CreateStudyTopicUseCase useCase,
    CancellationToken cancellationToken) =>
{
    var result = await useCase.ExecuteAsync(request, cancellationToken);

    return result.IsSuccess
        ? Results.Created($"/study-topics/{result.Value.Id}", result.Value)
        : ToErrorResult(result.Error);
});

studyTopics.MapPut("/{id:guid}", async (
    Guid id,
    UpdateStudyTopicRequest request,
    UpdateStudyTopicUseCase useCase,
    CancellationToken cancellationToken) =>
{
    var result = await useCase.ExecuteAsync(id, request, cancellationToken);

    return ToOkResult(result);
});

studyTopics.MapDelete("/{id:guid}", async (
    Guid id,
    DeleteStudyTopicUseCase useCase,
    CancellationToken cancellationToken) =>
{
    var result = await useCase.ExecuteAsync(id, cancellationToken);

    return ToNoContentResult(result);
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
        : ToErrorResult(result.Error);
}

static IResult ToOkResult<T>(Result<T> result)
{
    return result.IsSuccess
        ? Results.Ok(result.Value)
        : ToErrorResult(result.Error);
}

static IResult ToErrorResult(Error error)
{
    var response = new { error.Code, error.Message };

    return error.Code.EndsWith(".NotFound", StringComparison.Ordinal)
        ? Results.NotFound(response)
        : Results.BadRequest(response);
}

static Result<Guid> GetUserId(ClaimsPrincipal user)
{
    var value = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    return Guid.TryParse(value, out var userId)
        ? Result<Guid>.Success(userId)
        : Result<Guid>.Failure(new Error("Auth.InvalidToken", "Authenticated user id is missing or invalid."));
}
