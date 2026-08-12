using Microsoft.EntityFrameworkCore;
using StudyPlannerAgent.Application.Abstractions;
using StudyPlannerAgent.Application.Progress;
using StudyPlannerAgent.Application.StudyPlans;
using StudyPlannerAgent.Application.StudyTopics;
using StudyPlannerAgent.Infrastructure.Clock;
using StudyPlannerAgent.Infrastructure.Persistence.EfCore;
using StudyPlannerAgent.Infrastructure.Persistence.InMemory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<GetTodayStudyPlanUseCase>();
builder.Services.AddScoped<GetWeeklyStudyScheduleUseCase>();
builder.Services.AddScoped<GetStudyTopicsUseCase>();
builder.Services.AddScoped<GetStudyTopicByIdUseCase>();
builder.Services.AddScoped<CreateStudyTopicUseCase>();
builder.Services.AddScoped<UpdateStudyTopicUseCase>();
builder.Services.AddScoped<DeleteStudyTopicUseCase>();
builder.Services.AddScoped<RecordStudyProgressUseCase>();
builder.Services.AddScoped<GetProgressSummaryUseCase>();

builder.Services.AddSingleton<IClock, SystemClock>();

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
}

builder.Services
    .AddMcpServer()
    .WithHttpTransport(options =>
    {
        options.Stateless = true;
    })
    .WithTools<StudyPlannerAgent.McpServer.Tools.StudyPlannerTools>();

var app = builder.Build();

if (!string.IsNullOrWhiteSpace(normalizedConnectionString))
{
    await StudyPlannerDbInitializer.ApplyMigrationsAsync(app.Services);
}

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.MapMcp();

app.Run();
