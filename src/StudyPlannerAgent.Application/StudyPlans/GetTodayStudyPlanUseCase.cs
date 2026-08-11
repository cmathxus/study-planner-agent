using StudyPlannerAgent.Application.Abstractions;
using StudyPlannerAgent.Domain.Common;

namespace StudyPlannerAgent.Application.StudyPlans;

public sealed class GetTodayStudyPlanUseCase
{
    private readonly IClock _clock;
    private readonly IStudyProgressRepository _progressRepository;
    private readonly IStudyScheduleRepository _scheduleRepository;
    private readonly IStudyTopicRepository _topicRepository;

    public GetTodayStudyPlanUseCase(
        IClock clock,
        IStudyProgressRepository progressRepository,
        IStudyScheduleRepository scheduleRepository,
        IStudyTopicRepository topicRepository)
    {
        _clock = clock;
        _progressRepository = progressRepository;
        _scheduleRepository = scheduleRepository;
        _topicRepository = topicRepository;
    }

    public async Task<Result<IReadOnlyCollection<StudyPlanItemResponse>>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var schedules = await _scheduleRepository.GetByWeekdayAsync(_clock.Today.DayOfWeek, cancellationToken);
        var items = new List<StudyPlanItemResponse>();

        foreach (var schedule in schedules)
        {
            var topic = await _topicRepository.GetByIdAsync(schedule.StudyTopicId, cancellationToken);

            if (topic is null)
                continue;

            var progress = await _progressRepository.GetByTopicIdAsync(topic.Id, cancellationToken);
            var currentProgress = Math.Min(100, progress.Sum(entry => entry.Percentage.Value));

            items.Add(new StudyPlanItemResponse(
                topic.Id,
                topic.Name,
                topic.Description,
                schedule.Weekday,
                currentProgress));
        }

        return Result<IReadOnlyCollection<StudyPlanItemResponse>>.Success(items);
    }
}
