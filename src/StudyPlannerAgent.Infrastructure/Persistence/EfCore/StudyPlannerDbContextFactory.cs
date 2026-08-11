using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace StudyPlannerAgent.Infrastructure.Persistence.EfCore;

public sealed class StudyPlannerDbContextFactory : IDesignTimeDbContextFactory<StudyPlannerDbContext>
{
    public StudyPlannerDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Supabase")
            ?? "Host=localhost;Database=study_planner_agent;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<StudyPlannerDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new StudyPlannerDbContext(options);
    }
}
