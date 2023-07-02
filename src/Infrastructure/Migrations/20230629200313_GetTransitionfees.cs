using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetTransitionfees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"create PROCEDURE GetTransitionfees
                            @serviceID INT,
	                        @day DATETIME,
	                        @startTime TIME,
	                        @endTime TIME,
	                        @region INT,
	                        @resultId INT OUTPUT,
                            @resultPrice DECIMAL(18, 2) OUTPUT
                        WITH ENCRYPTION
                        AS
                        BEGIN
                            SET NOCOUNT ON;

                            DECLARE @serviceType INT;

                            CREATE TABLE #TempTable (OutputResult INT);
	                        CREATE TABLE #prices2 (
                                id INT,
                                price INT
	                        );

	                        CREATE TABLE #TempResourceTypeIDWithScheduleInvisible (
		                        ResourceTypeId INT,
		                        NoOfResourcesRequired INT
	                        );

                            INSERT INTO #TempTable
                            EXEC CheckServerTypeInBookingItem
                                @serviceID = @serviceID,
                                @OUTPUT = @serviceType OUTPUT;

                            IF (@serviceType = 1 OR @serviceType = 2)
                            BEGIN
                                INSERT INTO #TempResourceTypeIDWithScheduleInvisible (ResourceTypeId, NoOfResourcesRequired)
                                SELECT SM.[ResourceTypeId], SM.[NoOfResources]
                                FROM [dbo].[ServiceMetadata] AS SM
                                ,[dbo].[ResourceTypes] RT ,
		                        [dbo].[ScheduleItem] as SI,
		                        [dbo].[Resource] R,
		                        [dbo].[Schedule] S
                                WHERE SM.[ServiceId] = @serviceID
		                        and RT.[Id] = SM.[ResourceTypeId]
		                        and S.ResourceId=R.Id and SI.ScheduleId= s.ScheduleID
                                    AND [Shown] = 0
                                    AND [HasSchedual] = 1
                                    AND [Day] = @day
                                    AND [StartTime] = @startTime
                                    AND [EndTime] = @endTime
                                    AND [RegionId] = @region
                                    AND [Available] = 1;

                                SELECT TOP 1 @resultId = R.[Id], @resultPrice = R.[Price] * TRS.NoOfResourcesRequired
                                FROM [dbo].[Resource] R
                                INNER JOIN #TempResourceTypeIDWithScheduleInvisible TRS ON TRS.ResourceTypeId = R.[ResourceTypeId]
                                INNER JOIN [dbo].[ResourceSpecialCharacteristics] RSC ON R.[Id] = RSC.[ResourceID]
                                WHERE RSC.[AvailableCapacity] >= TRS.NoOfResourcesRequired;

                                INSERT INTO #prices2 (id, price)
                                SELECT @resultId, @resultPrice;

                            END
                            ELSE
                            BEGIN
                                SELECT 2;
                            END

                            DROP TABLE #TempTable;
                            DROP TABLE #TempResourceTypeIDWithScheduleInvisible;
                        END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROC GetTransitionfees");

        }
    }
}
