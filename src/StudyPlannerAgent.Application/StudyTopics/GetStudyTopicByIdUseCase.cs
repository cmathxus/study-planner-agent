using StudyPlannerAgent.Application.Abstractions;
using StudyPlannerAgent.Domain.Common;

namespace StudyPlannerAgent.Application.StudyTopics;

public sealed class GetStudyTopicByIdUseCase
{
    private readonly IStudyScheduleRepository _scheduleRepository;
    private readonly IStudyTopicRepository _topicRepository;

    public GetStudyTopicByIdUseCase(
        IStudyScheduleRepository scheduleRepository,
        IStudyTopicRepository topicRepository)
    {
        _scheduleRepository = scheduleRepository;
        _topicRepository = topicRepository;
    }

    public async Task<Result<StudyTopicResponse>> ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var topic = await _topicRepository.GetByIdAsync(id, cancellationToken);

        if (topic is null)
            return Result<StudyTopicResponse>.Failure(new Error("StudyTopic.NotFound", "Study topic was not found."));

        var schedule = await _scheduleRepository.GetByTopicIdAsync(id, cancellationToken);

        return Result<StudyTopicResponse>.Success(new StudyTopicResponse(
            topic.Id,
            topic.Name,
            topic.Description,
            schedule?.Weekday));
    }
}
