using Microsoft.EntityFrameworkCore;
using StudyPlannerAgent.Infrastructure.Persistence.EfCore.Records;

namespace StudyPlannerAgent.Infrastructure.Persistence.EfCore;

public sealed class StudyPlannerDbContext : DbContext
{
    public StudyPlannerDbContext(DbContextOptions<StudyPlannerDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserRecord> Users => Set<UserRecord>();
    public DbSet<StudyTopicRecord> StudyTopics => Set<StudyTopicRecord>();
    public DbSet<StudyScheduleRecord> StudySchedules => Set<StudyScheduleRecord>();
    public DbSet<StudyProgressEntryRecord> StudyProgressEntries => Set<StudyProgressEntryRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRecord>(builder =>
        {
            builder.ToTable("users");
            builder.HasKey(user => user.Id);
            builder.HasIndex(user => user.NormalizedEmail).IsUnique();
            builder.Property(user => user.Id).HasColumnName("id");
            builder.Property(user => user.Name).HasColumnName("name");
            builder.Property(user => user.Email).HasColumnName("email");
            builder.Property(user => user.NormalizedEmail).HasColumnName("normalized_email");
            builder.Property(user => user.PasswordHash).HasColumnName("password_hash");
            builder.Property(user => user.CreatedAt).HasColumnName("created_at");
            builder.Property(user => user.Name).HasMaxLength(200).IsRequired();
            builder.Property(user => user.Email).HasMaxLength(320).IsRequired();
            builder.Property(user => user.NormalizedEmail).HasMaxLength(320).IsRequired();
            builder.Property(user => user.PasswordHash).IsRequired();
            builder.Property(user => user.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<StudyTopicRecord>(builder =>
        {
            builder.ToTable("study_topics");
            builder.HasKey(topic => topic.Id);
            builder.Property(topic => topic.Id).HasColumnName("id");
            builder.Property(topic => topic.Name).HasColumnName("name");
            builder.Property(topic => topic.Description).HasColumnName("description");
            builder.Property(topic => topic.Name).HasMaxLength(200).IsRequired();
            builder.Property(topic => topic.Description).IsRequired();

            builder.HasData(SeedData.StudyTopics);
        });

        modelBuilder.Entity<StudyScheduleRecord>(builder =>
        {
            builder.ToTable("study_schedules");
            builder.HasKey(schedule => schedule.Id);
            builder.HasIndex(schedule => schedule.StudyTopicId).IsUnique();
            builder.Property(schedule => schedule.Id).HasColumnName("id");
            builder.Property(schedule => schedule.StudyTopicId).HasColumnName("study_topic_id");
            builder.Property(schedule => schedule.Weekday).HasColumnName("weekday").HasConversion<int>().IsRequired();

            builder
                .HasOne(schedule => schedule.StudyTopic)
                .WithMany(topic => topic.Schedules)
                .HasForeignKey(schedule => schedule.StudyTopicId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasData(SeedData.StudySchedules);
        });

        modelBuilder.Entity<StudyProgressEntryRecord>(builder =>
        {
            builder.ToTable("study_progress_entries");
            builder.HasKey(entry => entry.Id);
            builder.Property(entry => entry.Id).HasColumnName("id");
            builder.Property(entry => entry.UserId).HasColumnName("user_id");
            builder.Property(entry => entry.StudyTopicId).HasColumnName("study_topic_id");
            builder.Property(entry => entry.StudiedOn).HasColumnName("studied_on");
            builder.Property(entry => entry.Percentage).HasColumnName("percentage");
            builder.Property(entry => entry.Notes).HasColumnName("notes");
            builder.Property(entry => entry.CreatedAt).HasColumnName("created_at");
            builder.Property(entry => entry.StudiedOn).IsRequired();
            builder.Property(entry => entry.Percentage).IsRequired();
            builder.Property(entry => entry.CreatedAt).IsRequired();

            builder
                .HasOne(entry => entry.User)
                .WithMany(user => user.ProgressEntries)
                .HasForeignKey(entry => entry.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(entry => entry.StudyTopic)
                .WithMany(topic => topic.ProgressEntries)
                .HasForeignKey(entry => entry.StudyTopicId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
