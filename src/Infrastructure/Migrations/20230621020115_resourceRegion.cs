using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class resourceRegion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Resource_Regions_RegionId",
                table: "Resource");

            migrationBuilder.AlterColumn<int>(
                name: "RegionId",
                table: "Resource",
                type: "int",
                nullable: true,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Resource_Regions_RegionId",
                table: "Resource",
                column: "RegionId",
                principalTable: "Regions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Resource_Regions_RegionId",
                table: "Resource");

            migrationBuilder.AlterColumn<int>(
                name: "RegionId",
                table: "Resource",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Resource_Regions_RegionId",
                table: "Resource",
                column: "RegionId",
                principalTable: "Regions",
                principalColumn: "Id");
        }
    }
}
