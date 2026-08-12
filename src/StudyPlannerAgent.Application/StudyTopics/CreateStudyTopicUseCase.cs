using StudyPlannerAgent.Application.Abstractions;
using StudyPlannerAgent.Domain.Common;
using StudyPlannerAgent.Domain.Entities;

namespace StudyPlannerAgent.Application.StudyTopics;

public sealed class CreateStudyTopicUseCase
{
    private readonly IStudyScheduleRepository _scheduleRepository;
    private readonly IStudyTopicRepository _topicRepository;

    public CreateStudyTopicUseCase(
        IStudyScheduleRepository scheduleRepository,
        IStudyTopicRepository topicRepository)
    {
        _scheduleRepository = scheduleRepository;
        _topicRepository = topicRepository;
    }

    public async Task<Result<StudyTopicResponse>> ExecuteAsync(CreateStudyTopicRequest request, CancellationToken cancellationToken)
    {
        var topic = StudyTopic.Create(Guid.NewGuid(), request.Name, request.Description);

        if (topic.IsFailure)
            return Result<StudyTopicResponse>.Failure(topic.Error);

        var schedule = StudySchedule.Create(Guid.NewGuid(), topic.Value.Id, request.Weekday);

        if (schedule.IsFailure)
            return Result<StudyTopicResponse>.Failure(schedule.Error);

        await _topicRepository.AddAsync(topic.Value, cancellationToken);
        await _scheduleRepository.AddAsync(schedule.Value, cancellationToken);

        return Result<StudyTopicResponse>.Success(new StudyTopicResponse(
            topic.Value.Id,
            topic.Value.Name,
            topic.Value.Description,
            schedule.Value.Weekday));
    }
}
