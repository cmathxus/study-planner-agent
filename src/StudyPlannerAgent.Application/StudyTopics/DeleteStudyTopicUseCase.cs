using StudyPlannerAgent.Application.Abstractions;
using StudyPlannerAgent.Domain.Common;

namespace StudyPlannerAgent.Application.StudyTopics;

public sealed class DeleteStudyTopicUseCase
{
    private readonly IStudyScheduleRepository _scheduleRepository;
    private readonly IStudyTopicRepository _topicRepository;

    public DeleteStudyTopicUseCase(
        IStudyScheduleRepository scheduleRepository,
        IStudyTopicRepository topicRepository)
    {
        _scheduleRepository = scheduleRepository;
        _topicRepository = topicRepository;
    }

    public async Task<Result> ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var topic = await _topicRepository.GetByIdAsync(id, cancellationToken);

        if (topic is null)
            return Result.Failure(new Error("StudyTopic.NotFound", "Study topic was not found."));

        await _scheduleRepository.DeleteByTopicIdAsync(id, cancellationToken);
        await _topicRepository.DeleteAsync(id, cancellationToken);

        return Result.Success();
    }
}
