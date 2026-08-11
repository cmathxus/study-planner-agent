using Microsoft.EntityFrameworkCore;
using StudyPlannerAgent.Application.Abstractions;
using StudyPlannerAgent.Application.Progress;
using StudyPlannerAgent.Application.StudyPlans;
using StudyPlannerAgent.Infrastructure.Clock;
using StudyPlannerAgent.Infrastructure.Persistence.EfCore;
using StudyPlannerAgent.Infrastructure.Persistence.InMemory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<GetTodayStudyPlanUseCase>();
builder.Services.AddScoped<GetWeeklyStudyScheduleUseCase>();
builder.Services.AddScoped<RecordStudyProgressUseCase>();
builder.Services.AddScoped<GetProgressSummaryUseCase>();

builder.Services.AddSingleton<IClock, SystemClock>();

var connectionString = builder.Configuration.GetConnectionString("Supabase");

if (string.IsNullOrWhiteSpace(connectionString))
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
        options.UseNpgsql(connectionString);
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

if (!string.IsNullOrWhiteSpace(connectionString))
{
    await StudyPlannerDbInitializer.ApplyMigrationsAsync(app.Services);
}

app.MapMcp();

app.Run();
