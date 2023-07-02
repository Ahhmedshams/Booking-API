using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addProcGetHiddenCost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE PROCEDURE GetHiddenCost
                        @serviceID INT,
                        @resultId INT OUTPUT,
                        @resultPrice DECIMAL(18, 2) OUTPUT
                    WITH ENCRYPTION
                    AS
                    BEGIN
                        SET NOCOUNT ON;

                        DECLARE @serviceType INT;

                        CREATE TABLE #TempResourceTypeIDWithNoScheduleInvisible (
                            ResourceTypeId INT,
                            NoOfResourcesRequired INT
                        );

                        INSERT INTO #TempResourceTypeIDWithNoScheduleInvisible (ResourceTypeId, NoOfResourcesRequired)
                        SELECT SM.[ResourceTypeId], SM.[NoOfResources]
                        FROM [dbo].[ServiceMetadata] AS SM
                        INNER JOIN [dbo].[ResourceTypes] RT ON SM.[ServiceId] = @serviceID AND RT.[Id] = SM.[ResourceTypeId]
                        WHERE [Shown] = 0 AND [HasSchedual] = 0;

                        SELECT TOP 1 @resultId = R.[Id], @resultPrice = R.[Price] * TRS.NoOfResourcesRequired
                        FROM [dbo].[Resource] R
                        INNER JOIN #TempResourceTypeIDWithNoScheduleInvisible TRS ON TRS.ResourceTypeId = R.[ResourceTypeId]
                        INNER JOIN [dbo].[ResourceSpecialCharacteristics] RSC ON R.[Id] = RSC.[ResourceID]
                        WHERE RSC.[AvailableCapacity] >= TRS.NoOfResourcesRequired;

                        DROP TABLE #TempResourceTypeIDWithNoScheduleInvisible;
                    END
                    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROC GetHiddenCost");

        }
    }
}
