using ModelContextProtocol.Server;
using StudyPlannerAgent.Application.Progress;
using StudyPlannerAgent.Application.StudyPlans;
using StudyPlannerAgent.Application.StudyTopics;
using StudyPlannerAgent.Domain.Common;
using System.ComponentModel;

namespace StudyPlannerAgent.McpServer.Tools;

[McpServerToolType]
public sealed class StudyPlannerTools
{
    private readonly CreateStudyTopicUseCase _createStudyTopicUseCase;
    private readonly DeleteStudyTopicUseCase _deleteStudyTopicUseCase;
    private readonly GetProgressSummaryUseCase _getProgressSummaryUseCase;
    private readonly GetStudyTopicByIdUseCase _getStudyTopicByIdUseCase;
    private readonly GetStudyTopicsUseCase _getStudyTopicsUseCase;
    private readonly GetTodayStudyPlanUseCase _getTodayStudyPlanUseCase;
    private readonly GetWeeklyStudyScheduleUseCase _getWeeklyStudyScheduleUseCase;
    private readonly RecordStudyProgressUseCase _recordStudyProgressUseCase;
    private readonly UpdateStudyTopicUseCase _updateStudyTopicUseCase;

    public StudyPlannerTools(
        CreateStudyTopicUseCase createStudyTopicUseCase,
        DeleteStudyTopicUseCase deleteStudyTopicUseCase,
        GetProgressSummaryUseCase getProgressSummaryUseCase,
        GetStudyTopicByIdUseCase getStudyTopicByIdUseCase,
        GetStudyTopicsUseCase getStudyTopicsUseCase,
        GetTodayStudyPlanUseCase getTodayStudyPlanUseCase,
        GetWeeklyStudyScheduleUseCase getWeeklyStudyScheduleUseCase,
        RecordStudyProgressUseCase recordStudyProgressUseCase,
        UpdateStudyTopicUseCase updateStudyTopicUseCase)
    {
        _createStudyTopicUseCase = createStudyTopicUseCase;
        _deleteStudyTopicUseCase = deleteStudyTopicUseCase;
        _getProgressSummaryUseCase = getProgressSummaryUseCase;
        _getStudyTopicByIdUseCase = getStudyTopicByIdUseCase;
        _getStudyTopicsUseCase = getStudyTopicsUseCase;
        _getTodayStudyPlanUseCase = getTodayStudyPlanUseCase;
        _getWeeklyStudyScheduleUseCase = getWeeklyStudyScheduleUseCase;
        _recordStudyProgressUseCase = recordStudyProgressUseCase;
        _updateStudyTopicUseCase = updateStudyTopicUseCase;
    }

    [McpServerTool]
    [Description("Returns today's study plan with current progress for each scheduled topic.")]
    public async Task<object> GetTodayStudyPlan(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _getTodayStudyPlanUseCase.ExecuteAsync(userId, cancellationToken);

        return result.IsSuccess ? result.Value : result.Error;
    }

    [McpServerTool]
    [Description("Returns the full weekly study schedule.")]
    public async Task<object> GetWeeklyStudySchedule(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _getWeeklyStudyScheduleUseCase.ExecuteAsync(cancellationToken);

        return result.IsSuccess ? result.Value : result.Error;
    }

    [McpServerTool]
    [Description("Returns all study topics and their configured weekday.")]
    public async Task<object> GetStudyTopics(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _getStudyTopicsUseCase.ExecuteAsync(cancellationToken);

        return result.IsSuccess ? result.Value : result.Error;
    }

    [McpServerTool]
    [Description("Returns a study topic by id.")]
    public async Task<object> GetStudyTopicById(Guid userId, Guid topicId, CancellationToken cancellationToken)
    {
        var result = await _getStudyTopicByIdUseCase.ExecuteAsync(topicId, cancellationToken);

        return result.IsSuccess ? result.Value : result.Error;
    }

    [McpServerTool]
    [Description("Creates a study topic and assigns it to a weekday. Valid weekdays: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday.")]
    public async Task<object> CreateStudyTopic(
        Guid userId,
        string name,
        string description,
        string weekday,
        CancellationToken cancellationToken)
    {
        var parsedWeekday = ParseWeekday(weekday);

        if (parsedWeekday.IsFailure)
            return parsedWeekday.Error;

        var result = await _createStudyTopicUseCase.ExecuteAsync(
            new CreateStudyTopicRequest(name, description, parsedWeekday.Value),
            cancellationToken);

        return result.IsSuccess ? result.Value : result.Error;
    }

    [McpServerTool]
    [Description("Updates a study topic and its weekday. Valid weekdays: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday.")]
    public async Task<object> UpdateStudyTopic(
        Guid userId,
        Guid topicId,
        string name,
        string description,
        string weekday,
        CancellationToken cancellationToken)
    {
        var parsedWeekday = ParseWeekday(weekday);

        if (parsedWeekday.IsFailure)
            return parsedWeekday.Error;

        var result = await _updateStudyTopicUseCase.ExecuteAsync(
            topicId,
            new UpdateStudyTopicRequest(name, description, parsedWeekday.Value),
            cancellationToken);

        return result.IsSuccess ? result.Value : result.Error;
    }

    [McpServerTool]
    [Description("Deletes a study topic by id.")]
    public async Task<object> DeleteStudyTopic(Guid userId, Guid topicId, CancellationToken cancellationToken)
    {
        var result = await _deleteStudyTopicUseCase.ExecuteAsync(topicId, cancellationToken);

        return result.IsSuccess ? "Study topic deleted." : result.Error;
    }

    [McpServerTool]
    [Description("Records a study progress entry. The percentage must be at least 20.")]
    public async Task<object> RecordStudyProgress(Guid userId, Guid topicId, int percentage, string? notes, CancellationToken cancellationToken)
    {
        var result = await _recordStudyProgressUseCase.ExecuteAsync(
            userId,
            new RecordProgressRequest(topicId, percentage, notes),
            cancellationToken);

        return result.IsSuccess ? "Progress recorded." : result.Error;
    }

    [McpServerTool]
    [Description("Returns a progress summary grouped by study topic.")]
    public async Task<object> GetProgressSummary(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _getProgressSummaryUseCase.ExecuteAsync(userId, cancellationToken);

        return result.IsSuccess ? result.Value : result.Error;
    }

    private static Result<DayOfWeek> ParseWeekday(string weekday)
    {
        return Enum.TryParse<DayOfWeek>(weekday, ignoreCase: true, out var parsedWeekday)
            ? Result<DayOfWeek>.Success(parsedWeekday)
            : Result<DayOfWeek>.Failure(new Error(
                "StudySchedule.InvalidWeekday",
                "Weekday must be one of: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday."));
    }
}
