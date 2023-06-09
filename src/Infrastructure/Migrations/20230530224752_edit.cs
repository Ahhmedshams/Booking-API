using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class edit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ScheduleItem",
                table: "ScheduleItem");

            migrationBuilder.RenameColumn(
                name: "Time",
                table: "ClientBookings",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "Duration",
                table: "ClientBookings",
                newName: "EndTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Day",
                table: "ScheduleItem",
                type: "DATE",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<bool>(
                name: "Shift",
                table: "ScheduleItem",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCost",
                table: "ClientBookings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScheduleItem",
                table: "ScheduleItem",
                columns: new[] { "Day", "StartTime", "EndTime", "ScheduleId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ScheduleItem",
                table: "ScheduleItem");

            migrationBuilder.DropColumn(
                name: "Shift",
                table: "ScheduleItem");

            migrationBuilder.DropColumn(
                name: "TotalCost",
                table: "ClientBookings");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "ClientBookings",
                newName: "Time");

            migrationBuilder.RenameColumn(
                name: "EndTime",
                table: "ClientBookings",
                newName: "Duration");

            migrationBuilder.AlterColumn<string>(
                name: "Day",
                table: "ScheduleItem",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "DATE");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScheduleItem",
                table: "ScheduleItem",
                columns: new[] { "Day", "StartTime", "EndTime" });
        }
    }
}
