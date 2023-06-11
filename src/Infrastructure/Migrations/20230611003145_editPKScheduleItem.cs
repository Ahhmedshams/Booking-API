using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class editPKScheduleItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ScheduleItem",
                table: "ScheduleItem");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleItem_ScheduleId",
                table: "ScheduleItem");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScheduleItem",
                table: "ScheduleItem",
                columns: new[] { "ScheduleId", "Day", "StartTime", "EndTime" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ScheduleItem",
                table: "ScheduleItem");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScheduleItem",
                table: "ScheduleItem",
                columns: new[] { "Day", "StartTime", "EndTime" });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleItem_ScheduleId",
                table: "ScheduleItem",
                column: "ScheduleId");
        }
    }
}
