using System.Text.Json;
using StudyPlannerAgent.Application;
using StudyPlannerAgent.Application.Progress;
using StudyPlannerAgent.Application.StudyPlans;
using StudyPlannerAgent.Domain.Common;
using StudyPlannerAgent.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddOpenApi();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var studyPlan = app.MapGroup("/study-plan");

studyPlan.MapGet("/today", async (GetTodayStudyPlanUseCase useCase, CancellationToken cancellationToken) =>
{
    var result = await useCase.ExecuteAsync(cancellationToken);

    return ToOkResult(result);
});

studyPlan.MapGet("/week", async (GetWeeklyStudyScheduleUseCase useCase, CancellationToken cancellationToken) =>
{
    var result = await useCase.ExecuteAsync(cancellationToken);

    return ToOkResult(result);
});

app.MapPost("/progress", async (
    RecordProgressRequest request,
    RecordStudyProgressUseCase useCase,
    CancellationToken cancellationToken) =>
{
    var result = await useCase.ExecuteAsync(request, cancellationToken);

    return ToNoContentResult(result);
});

app.MapGet("/progress/summary", async (GetProgressSummaryUseCase useCase, CancellationToken cancellationToken) =>
{
    var result = await useCase.ExecuteAsync(cancellationToken);

    return ToOkResult(result);
});

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
