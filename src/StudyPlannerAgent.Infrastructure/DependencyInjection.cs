using Microsoft.Extensions.DependencyInjection;
using StudyPlannerAgent.Application.Abstractions;
using StudyPlannerAgent.Infrastructure.Clock;
using StudyPlannerAgent.Infrastructure.Persistence.InMemory;

namespace StudyPlannerAgent.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<InMemoryStudyData>();
        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<IStudyTopicRepository, InMemoryStudyTopicRepository>();
        services.AddSingleton<IStudyScheduleRepository, InMemoryStudyScheduleRepository>();
        services.AddSingleton<IStudyProgressRepository, InMemoryStudyProgressRepository>();

        return services;
    }
}
