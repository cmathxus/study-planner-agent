using ModelContextProtocol.Server;
using StudyPlannerAgent.Application.Progress;
using StudyPlannerAgent.Application.StudyPlans;
using System.ComponentModel;

namespace StudyPlannerAgent.McpServer.Tools;

[McpServerToolType]
public sealed class StudyPlannerTools
{
    private readonly GetProgressSummaryUseCase _getProgressSummaryUseCase;
    private readonly GetTodayStudyPlanUseCase _getTodayStudyPlanUseCase;
    private readonly GetWeeklyStudyScheduleUseCase _getWeeklyStudyScheduleUseCase;
    private readonly RecordStudyProgressUseCase _recordStudyProgressUseCase;

    public StudyPlannerTools(
        GetProgressSummaryUseCase getProgressSummaryUseCase,
        GetTodayStudyPlanUseCase getTodayStudyPlanUseCase,
        GetWeeklyStudyScheduleUseCase getWeeklyStudyScheduleUseCase,
        RecordStudyProgressUseCase recordStudyProgressUseCase)
    {
        _getProgressSummaryUseCase = getProgressSummaryUseCase;
        _getTodayStudyPlanUseCase = getTodayStudyPlanUseCase;
        _getWeeklyStudyScheduleUseCase = getWeeklyStudyScheduleUseCase;
        _recordStudyProgressUseCase = recordStudyProgressUseCase;
    }

    [McpServerTool]
    [Description("Returns today's study plan with current progress for each scheduled topic.")]
    public async Task<object> GetTodayStudyPlan(CancellationToken cancellationToken)
    {
        var result = await _getTodayStudyPlanUseCase.ExecuteAsync(cancellationToken);

        return result.IsSuccess ? result.Value : result.Error;
    }

    [McpServerTool]
    [Description("Returns the full weekly study schedule.")]
    public async Task<object> GetWeeklyStudySchedule(CancellationToken cancellationToken)
    {
        var result = await _getWeeklyStudyScheduleUseCase.ExecuteAsync(cancellationToken);

        return result.IsSuccess ? result.Value : result.Error;
    }

    [McpServerTool]
    [Description("Records a study progress entry. The percentage must be at least 20.")]
    public async Task<object> RecordStudyProgress(Guid topicId, int percentage, string? notes, CancellationToken cancellationToken)
    {
        var result = await _recordStudyProgressUseCase.ExecuteAsync(
            new RecordProgressRequest(topicId, percentage, notes),
            cancellationToken);

        return result.IsSuccess ? "Progress recorded." : result.Error;
    }

    [McpServerTool]
    [Description("Returns a progress summary grouped by study topic.")]
    public async Task<object> GetProgressSummary(CancellationToken cancellationToken)
    {
        var result = await _getProgressSummaryUseCase.ExecuteAsync(cancellationToken);

        return result.IsSuccess ? result.Value : result.Error;
    }
}
