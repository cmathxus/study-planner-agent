using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace StudyPlannerAgent.Infrastructure.Persistence.EfCore;

public static class StudyPlannerDbInitializer
{
    public static async Task ApplyMigrationsAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StudyPlannerDbContext>();

        await dbContext.Database.MigrateAsync();
    }
}
