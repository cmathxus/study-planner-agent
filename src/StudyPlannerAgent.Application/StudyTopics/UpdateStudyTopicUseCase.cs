using StudyPlannerAgent.Application.Abstractions;
using StudyPlannerAgent.Domain.Common;
using StudyPlannerAgent.Domain.Entities;

namespace StudyPlannerAgent.Application.StudyTopics;

public sealed class UpdateStudyTopicUseCase
{
    private readonly IStudyScheduleRepository _scheduleRepository;
    private readonly IStudyTopicRepository _topicRepository;

    public UpdateStudyTopicUseCase(
        IStudyScheduleRepository scheduleRepository,
        IStudyTopicRepository topicRepository)
    {
        _scheduleRepository = scheduleRepository;
        _topicRepository = topicRepository;
    }

    public async Task<Result<StudyTopicResponse>> ExecuteAsync(Guid id, UpdateStudyTopicRequest request, CancellationToken cancellationToken)
    {
        var currentTopic = await _topicRepository.GetByIdAsync(id, cancellationToken);

        if (currentTopic is null)
            return Result<StudyTopicResponse>.Failure(new Error("StudyTopic.NotFound", "Study topic was not found."));

        var topic = StudyTopic.Create(id, request.Name, request.Description);

        if (topic.IsFailure)
            return Result<StudyTopicResponse>.Failure(topic.Error);

        var currentSchedule = await _scheduleRepository.GetByTopicIdAsync(id, cancellationToken);
        var schedule = StudySchedule.Create(currentSchedule?.Id ?? Guid.NewGuid(), id, request.Weekday);

        if (schedule.IsFailure)
            return Result<StudyTopicResponse>.Failure(schedule.Error);

        await _topicRepository.UpdateAsync(topic.Value, cancellationToken);

        if (currentSchedule is null)
            await _scheduleRepository.AddAsync(schedule.Value, cancellationToken);
        else
            await _scheduleRepository.UpdateAsync(schedule.Value, cancellationToken);

        return Result<StudyTopicResponse>.Success(new StudyTopicResponse(
            topic.Value.Id,
            topic.Value.Name,
            topic.Value.Description,
            schedule.Value.Weekday));
    }
}
