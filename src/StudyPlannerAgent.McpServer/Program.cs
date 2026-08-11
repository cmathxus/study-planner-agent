using Npgsql;
using StudyPlannerAgent.Application.Abstractions;
using StudyPlannerAgent.Application.Progress;
using StudyPlannerAgent.Application.StudyPlans;
using StudyPlannerAgent.Infrastructure.Clock;
using StudyPlannerAgent.Infrastructure.Persistence.InMemory;
using StudyPlannerAgent.Infrastructure.Persistence.Postgres;

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
    builder.Services.AddSingleton(_ => NpgsqlDataSource.Create(connectionString));
    builder.Services.AddScoped<IStudyTopicRepository, PostgresStudyTopicRepository>();
    builder.Services.AddScoped<IStudyScheduleRepository, PostgresStudyScheduleRepository>();
    builder.Services.AddScoped<IStudyProgressRepository, PostgresStudyProgressRepository>();
}

builder.Services
    .AddMcpServer()
    .WithHttpTransport(options =>
    {
        options.Stateless = true;
    })
    .WithTools<StudyPlannerAgent.McpServer.Tools.StudyPlannerTools>();

var app = builder.Build();

app.MapMcp();

app.Run();
