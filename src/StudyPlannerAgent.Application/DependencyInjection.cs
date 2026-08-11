using Microsoft.Extensions.DependencyInjection;
using StudyPlannerAgent.Application.Progress;
using StudyPlannerAgent.Application.StudyPlans;

namespace StudyPlannerAgent.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<GetTodayStudyPlanUseCase>();
        services.AddScoped<GetWeeklyStudyScheduleUseCase>();
        services.AddScoped<RecordStudyProgressUseCase>();
        services.AddScoped<GetProgressSummaryUseCase>();

        return services;
    }
}
