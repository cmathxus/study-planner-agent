using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using StudyPlannerAgent.Application.Abstractions;
using StudyPlannerAgent.Infrastructure.Clock;
using StudyPlannerAgent.Infrastructure.Persistence.InMemory;
using StudyPlannerAgent.Infrastructure.Persistence.Postgres;

namespace StudyPlannerAgent.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IClock, SystemClock>();

        var connectionString = configuration.GetConnectionString("Supabase");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            services.AddSingleton<InMemoryStudyData>();
            services.AddSingleton<IStudyTopicRepository, InMemoryStudyTopicRepository>();
            services.AddSingleton<IStudyScheduleRepository, InMemoryStudyScheduleRepository>();
            services.AddSingleton<IStudyProgressRepository, InMemoryStudyProgressRepository>();

            return services;
        }

        services.AddSingleton(_ => NpgsqlDataSource.Create(connectionString));
        services.AddScoped<IStudyTopicRepository, PostgresStudyTopicRepository>();
        services.AddScoped<IStudyScheduleRepository, PostgresStudyScheduleRepository>();
        services.AddScoped<IStudyProgressRepository, PostgresStudyProgressRepository>();

        return services;
    }
}
