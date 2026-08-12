using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyPlannerAgent.Infrastructure.Persistence.EfCore.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueStudyTopicSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_study_schedules_study_topic_id",
                table: "study_schedules");

            migrationBuilder.CreateIndex(
                name: "IX_study_schedules_study_topic_id",
                table: "study_schedules",
                column: "study_topic_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_study_schedules_study_topic_id",
                table: "study_schedules");

            migrationBuilder.CreateIndex(
                name: "IX_study_schedules_study_topic_id",
                table: "study_schedules",
                column: "study_topic_id");
        }
    }
}
