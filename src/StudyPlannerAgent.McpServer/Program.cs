using StudyPlannerAgent.Application;
using StudyPlannerAgent.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure();
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
