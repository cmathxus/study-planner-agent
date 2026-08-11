using StudyPlannerAgent.Application.Abstractions;
using StudyPlannerAgent.Domain.Common;
using StudyPlannerAgent.Domain.Entities;

namespace StudyPlannerAgent.Application.Progress;

public sealed class RecordStudyProgressUseCase
{
    private readonly IClock _clock;
    private readonly IStudyProgressRepository _progressRepository;
    private readonly IStudyTopicRepository _topicRepository;

    public RecordStudyProgressUseCase(
        IClock clock,
        IStudyProgressRepository progressRepository,
        IStudyTopicRepository topicRepository)
    {
        _clock = clock;
        _progressRepository = progressRepository;
        _topicRepository = topicRepository;
    }

    public async Task<Result> ExecuteAsync(RecordProgressRequest request, CancellationToken cancellationToken)
    {
        var topic = await _topicRepository.GetByIdAsync(request.TopicId, cancellationToken);

        if (topic is null)
            return Result.Failure(new Error("StudyTopic.NotFound", "Study topic was not found."));

        var progressEntry = StudyProgressEntry.Create(
            Guid.NewGuid(),
            request.TopicId,
            _clock.Today,
            request.Percentage,
            request.Notes);

        if (progressEntry.IsFailure)
            return Result.Failure(progressEntry.Error);

        await _progressRepository.AddAsync(progressEntry.Value, cancellationToken);

        return Result.Success();
    }
}
