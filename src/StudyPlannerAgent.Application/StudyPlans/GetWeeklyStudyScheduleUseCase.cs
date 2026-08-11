using StudyPlannerAgent.Application.Abstractions;
using StudyPlannerAgent.Domain.Common;

namespace StudyPlannerAgent.Application.StudyPlans;

public sealed class GetWeeklyStudyScheduleUseCase
{
    private readonly IStudyScheduleRepository _scheduleRepository;
    private readonly IStudyTopicRepository _topicRepository;

    public GetWeeklyStudyScheduleUseCase(
        IStudyScheduleRepository scheduleRepository,
        IStudyTopicRepository topicRepository)
    {
        _scheduleRepository = scheduleRepository;
        _topicRepository = topicRepository;
    }

    public async Task<Result<IReadOnlyCollection<StudyPlanItemResponse>>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var schedules = await _scheduleRepository.GetAllAsync(cancellationToken);
        var topics = await _topicRepository.GetAllAsync(cancellationToken);

        var items = schedules
            .Join(
                topics,
                schedule => schedule.StudyTopicId,
                topic => topic.Id,
                (schedule, topic) => new StudyPlanItemResponse(topic.Id, topic.Name, topic.Description, schedule.Weekday, 0))
            .OrderBy(item => item.Weekday)
            .ToList();

        return Result<IReadOnlyCollection<StudyPlanItemResponse>>.Success(items);
    }
}
