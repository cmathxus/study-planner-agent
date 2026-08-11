using StudyPlannerAgent.Application.Abstractions;
using StudyPlannerAgent.Domain.Common;

namespace StudyPlannerAgent.Application.Progress;

public sealed class GetProgressSummaryUseCase
{
    private readonly IStudyProgressRepository _progressRepository;
    private readonly IStudyTopicRepository _topicRepository;

    public GetProgressSummaryUseCase(
        IStudyProgressRepository progressRepository,
        IStudyTopicRepository topicRepository)
    {
        _progressRepository = progressRepository;
        _topicRepository = topicRepository;
    }

    public async Task<Result<IReadOnlyCollection<ProgressSummaryItemResponse>>> ExecuteAsync(Guid userId, CancellationToken cancellationToken)
    {
        var topics = await _topicRepository.GetAllAsync(cancellationToken);
        var progressEntries = await _progressRepository.GetAllByUserIdAsync(userId, cancellationToken);

        var summary = topics
            .Select(topic =>
            {
                var currentProgress = progressEntries
                    .Where(entry => entry.StudyTopicId == topic.Id)
                    .Sum(entry => entry.Percentage.Value);

                return new ProgressSummaryItemResponse(topic.Id, topic.Name, Math.Min(100, currentProgress));
            })
            .ToList();

        return Result<IReadOnlyCollection<ProgressSummaryItemResponse>>.Success(summary);
    }
}
