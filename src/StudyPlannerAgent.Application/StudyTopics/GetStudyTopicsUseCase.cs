using StudyPlannerAgent.Application.Abstractions;
using StudyPlannerAgent.Domain.Common;

namespace StudyPlannerAgent.Application.StudyTopics;

public sealed class GetStudyTopicsUseCase
{
    private readonly IStudyScheduleRepository _scheduleRepository;
    private readonly IStudyTopicRepository _topicRepository;

    public GetStudyTopicsUseCase(
        IStudyScheduleRepository scheduleRepository,
        IStudyTopicRepository topicRepository)
    {
        _scheduleRepository = scheduleRepository;
        _topicRepository = topicRepository;
    }

    public async Task<Result<IReadOnlyCollection<StudyTopicResponse>>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var topics = await _topicRepository.GetAllAsync(cancellationToken);
        var schedules = await _scheduleRepository.GetAllAsync(cancellationToken);
        var scheduleByTopicId = schedules.ToDictionary(schedule => schedule.StudyTopicId);

        var response = topics
            .Select(topic =>
            {
                scheduleByTopicId.TryGetValue(topic.Id, out var schedule);

                return new StudyTopicResponse(
                    topic.Id,
                    topic.Name,
                    topic.Description,
                    schedule?.Weekday);
            })
            .OrderBy(topic => topic.Weekday)
            .ThenBy(topic => topic.Name)
            .ToList();

        return Result<IReadOnlyCollection<StudyTopicResponse>>.Success(response);
    }
}
