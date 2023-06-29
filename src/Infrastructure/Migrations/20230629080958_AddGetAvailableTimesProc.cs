using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGetAvailableTimesProc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"Create proc GetAvailableTimes  @date date,@serviceID int ,@RegionId int = null
as
BEGIN try --1
IF EXISTS (
    SELECT [ServiceId]
    FROM [dbo].[ServiceMetadata]
    WHERE [ServiceId] = @serviceID
)
BEGIN ---2
set nocount on ;
declare @resProc1 int 
declare @resProc2 int 
declare @resProc3 int 
EXEC @resProc1= proc1 @serviceID ;
EXEC @resProc2= proc2 @serviceID ;
EXEC @resProc3= proc3 @serviceID ;


create TABLE #TempResourceTypeID (
    ResourceTypeId INT,
    NoOfResourcesRequired INT
);
	INSERT INTO #TempResourceTypeID (ResourceTypeId, NoOfResourcesRequired)
	SELECT [ResourceTypeId], NoOfResources
	FROM [dbo].[ServiceMetadata]
	WHERE [ServiceId] = @serviceID;

	--select * from #TempResourceTypeID
		--type1  has schedule and shown 
		--type2  has schedule and invisible 
		--type3  has Noschedule and invisible 
    IF 
    (@resProc1 =1 and @resProc2 =1 and @resProc3 =1)  --case if service has all types  first ,second ,third
    BEGIN  --3
		IF EXISTS			
				(--case1 shown 1 schedule 1
				SELECT 1
				FROM [dbo].[ResourceTypes],
				#TempResourceTypeID 
				WHERE [Id] = ResourceTypeId 
				AND [shown] = 1 
				AND [HasSchedual]=1
				)
				BEGIN  --4

						IF EXISTS 
						(--case 2  shown 0 schedule 1
						SELECT 1 
							 FROM [dbo].[ResourceTypes] RT,
							 #TempResourceTypeID TS,
							 [dbo].[ResourceSpecialCharacteristics]RSC,
							 [dbo].[Resource] R,
							 [dbo].[ScheduleItem] SI,
							 [dbo].[Schedule] S
							 WHERE RT.[Id] =TS.ResourceTypeId 
							 AND RSC.[ResourceID]=R.[Id]
							 AND R.[ResourceTypeId]=TS.ResourceTypeId
							 AND SI.[ScheduleId]=S.[ScheduleID]
							 AND S.[ResourceId]=RSC.[ResourceID]
							 AND SI.[ID]=RSC.[ScheduleID]
							 AND [shown] = 0 
							 AND [HasSchedual]=1
							 AND NoOfResourcesRequired<=RSC.AvailableCapacity 
							 AND SI.[Available]=1
							 AND @date =[Day]
							-- AND SI.[StartTime]= @startTime 
							-- AND SI.[EndTime]= @endTime
							 and S.[IsDeleted]=0
							 and SI.[IsDeleted]=0
						 )
						 BEGIN --5

							IF EXISTS 
							(--case 3   shown 0 schedule 0
							 SELECT 1 
							 FROM [dbo].[ResourceTypes] RT,#TempResourceTypeID TS,
							 [dbo].[ResourceSpecialCharacteristics]RSC,[dbo].[Resource] R
							 WHERE RT.[Id] =TS.ResourceTypeId 
							 and RSC.[ResourceID]=R.[Id]
							 and R.[ResourceTypeId]=TS.ResourceTypeId
							 AND [shown] = 0 
							 AND [HasSchedual]=0
							 AND NoOfResourcesRequired<=[AvailableCapacity])
							 BEGIN  --6
							 IF @RegionId IS NOT NULL
								 BEGIN
									select  SI.*
									from scheduleitem  as SI , 
									Schedule as S,
									[dbo].[Resource] R,
									#TempResourceTypeID,
									[dbo].[ResourceTypes] RT
									where 
									S.[ResourceId]=R.[Id]
									and  SI.scheduleId= S.scheduleId
									AND #TempResourceTypeID.[ResourceTypeId]=RT.[Id]
									AND R.[ResourceTypeId]=RT.[Id]
									AND SI.[Day]=@DATE
								--	AND SI.[StartTime]= @startTime 
								--	AND SI.[EndTime]= @endTime
									and SI.available= 1 
									and S.[IsDeleted]=0
									and SI.[IsDeleted]=0
									AND [Shown]=1
									AND [HasSchedual]=1
									AND @date BETWEEN S.[FromDate] AND S.[ToDate]
									AND RegionId = @regionId
									--GROUP BY SI.[StartTime], SI.[EndTime]

								END
							ELSE
								BEGIN
									select SI.*
									from scheduleitem  as SI , 
									Schedule as S,
									[dbo].[Resource] R,
									#TempResourceTypeID,
									[dbo].[ResourceTypes] RT
									where 
									S.[ResourceId]=R.[Id]
									and  SI.scheduleId= S.scheduleId
									AND #TempResourceTypeID.[ResourceTypeId]=RT.[Id]
									AND R.[ResourceTypeId]=RT.[Id]
									AND SI.[Day]=@DATE
								--	AND SI.[StartTime]= @startTime 
								--	AND SI.[EndTime]= @endTime
									and SI.available= 1 
									and S.[IsDeleted]=0
									and SI.[IsDeleted]=0
									AND [Shown]=1
									AND [HasSchedual]=1
									AND @date BETWEEN S.[FromDate] AND S.[ToDate]
									--GROUP BY SI.[StartTime], SI.[EndTime]

								END
						 END --6
				  END --5

		    END --4
				
     END --3
			 
		--type1  has schedule and shown 
		--type2  has schedule and invisible 
	
	ELSE IF (@resProc1 =1 and @resProc2 =1 and @resProc3=0 )--case if service has 2 types  first ,second 
	begin  --7

				IF EXISTS			
				(--case1 shown 1 schedule 1
				SELECT 1
				FROM [dbo].[ResourceTypes],
				#TempResourceTypeID 
				WHERE [Id] = ResourceTypeId 
				AND [shown] = 1 
				AND [HasSchedual]=1
				)
				BEGIN --8

						IF EXISTS 
						(--case 2  shown 0 schedule 1
						SELECT 1 
							 FROM [dbo].[ResourceTypes] RT,
							 #TempResourceTypeID TS,
							 [dbo].[ResourceSpecialCharacteristics]RSC,
							 [dbo].[Resource] R,
							 [dbo].[ScheduleItem] SI,
							 [dbo].[Schedule] S
							 WHERE RT.[Id] =TS.ResourceTypeId 
							 AND RSC.[ResourceID]=R.[Id]
							 AND R.[ResourceTypeId]=TS.ResourceTypeId
							 AND SI.[ScheduleId]=S.[ScheduleID]
							 AND S.[ResourceId]=RSC.[ResourceID]
							 AND SI.[ID]=RSC.[ScheduleID]
							 AND [shown] = 0 
							 AND [HasSchedual]=1
							 AND NoOfResourcesRequired<=RSC.AvailableCapacity 
							 AND SI.[Available]=1
							 AND SI.[Day]=@date
						--	 AND SI.[StartTime]= @startTime 
						--	 AND SI.[EndTime]= @endTime
							 and S.[IsDeleted]=0
							 and SI.[IsDeleted]=0
							 AND @date BETWEEN S.[FromDate] AND S.[ToDate]

						 )
						 
							 BEGIN --9
							 IF @RegionId IS NOT NULL
								BEGIN
									select SI.*
									from scheduleitem  as SI , 
									Schedule as S,
									[dbo].[Resource] R,
									#TempResourceTypeID,
									[dbo].[ResourceTypes] RT
									where 
									S.[ResourceId]=R.[Id]
									and  SI.scheduleId= S.scheduleId
									AND #TempResourceTypeID.[ResourceTypeId]=RT.[Id]
									AND R.[ResourceTypeId]=RT.[Id]
									AND SI.[Day]=@date
							--		AND SI.[StartTime]= @startTime 
							--		AND SI.[EndTime]= @endTime
									and available= 1 
									and S.[IsDeleted]=0
									and SI.[IsDeleted]=0
									AND [Shown]=1
									AND [HasSchedual]=1
									AND @date BETWEEN S.[FromDate] AND S.[ToDate]
									AND RegionId = @regionId
									GROUP BY SI.[StartTime], SI.[EndTime]

								END
							ELSE
								BEGIN
									select SI.*
									from scheduleitem  as SI , 
									Schedule as S,
									[dbo].[Resource] R,
									#TempResourceTypeID,
									[dbo].[ResourceTypes] RT
									where 
									S.[ResourceId]=R.[Id]
									and  SI.scheduleId= S.scheduleId
									AND #TempResourceTypeID.[ResourceTypeId]=RT.[Id]
									AND R.[ResourceTypeId]=RT.[Id]
									AND SI.[Day]=@date
								--	AND SI.[StartTime]= @startTime 
								--	AND SI.[EndTime]= @endTime
									and available= 1 
									and S.[IsDeleted]=0
									and SI.[IsDeleted]=0
									AND [Shown]=1
									AND [HasSchedual]=1
									AND @date BETWEEN S.[FromDate] AND S.[ToDate]
									GROUP BY SI.[StartTime], SI.[EndTime]
								END
							END --9
				END  --8
		END--7

		--type1  has schedule and shown 
		--type3  has Noschedule and invisible 
	ELSE IF (@resProc1 =1and @resProc3 =1 and @resProc2=0 )--case if service has two types  first ,third
	begin  --10
				IF EXISTS			
				(--case1 shown 1 schedule 1
				SELECT 1
				FROM [dbo].[ResourceTypes],
				#TempResourceTypeID 
				WHERE [Id] = ResourceTypeId 
				AND [shown] = 1 
				AND [HasSchedual]=1
				)
				BEGIN --11
							IF EXISTS 
							(--case 3   shown 0 schedule 0
							 SELECT 1 
							 FROM [dbo].[ResourceTypes] RT,#TempResourceTypeID TS,
							 [dbo].[ResourceSpecialCharacteristics]RSC,[dbo].[Resource] R
							 WHERE RT.[Id] =TS.ResourceTypeId 
							 and RSC.[ResourceID]=R.[Id]
							 and R.[ResourceTypeId]=TS.ResourceTypeId
							 AND [shown] = 0 
							 AND [HasSchedual]=0
							 AND NoOfResourcesRequired<=RSC.[AvailableCapacity])
							 BEGIN --12
								select SI.*
								from scheduleitem  as SI , 
								Schedule as S,
								[dbo].[Resource] R,
								#TempResourceTypeID,
								[dbo].[ResourceTypes] RT
								where 
								S.[ResourceId]=R.[Id]
							    and  SI.scheduleId= S.scheduleId
								AND #TempResourceTypeID.[ResourceTypeId]=RT.[Id]
								AND R.[ResourceTypeId]=RT.[Id]
								AND SI.[Day]=@DATE
							--	AND SI.[StartTime]= @startTime 
							--	AND SI.[EndTime]= @endTime
								and available= 1 
								and S.[IsDeleted]=0
								and SI.[IsDeleted]=0
								AND [Shown]=1
								AND [HasSchedual]=1
								AND @date BETWEEN S.[FromDate] AND S.[ToDate]
								--GROUP BY SI.[StartTime], SI.[EndTime]

							END --12
						END --11
				END --10
			ELSE IF (@resProc1 =1and @resProc2=0  and @resProc3=0)--case if service has one type  first type
	        begin  --13
				IF EXISTS			
				(--case1 shown 1 schedule 1
				SELECT 1
				FROM [dbo].[ResourceTypes],
				#TempResourceTypeID 
				WHERE [Id] = ResourceTypeId 
				AND [shown] = 1 
				AND [HasSchedual]=1
				)
				BEGIN --14
						IF @RegionId IS NOT NULL
						BEGIN
							select SI.*
							from scheduleitem  as SI , 
							Schedule as S,
							[dbo].[Resource] R,
							#TempResourceTypeID,
							[dbo].[ResourceTypes] RT
							where 
							S.[ResourceId]=R.[Id]
							and  SI.scheduleId= S.scheduleId
							AND #TempResourceTypeID.[ResourceTypeId]=RT.[Id]
							AND R.[ResourceTypeId]=RT.[Id]
							AND SI.[Day] =@date
						--	AND SI.[StartTime]= @startTime 
						--	AND SI.[EndTime]= @endTime
							and available= 1 
							and S.[IsDeleted]=0
							and SI.[IsDeleted]=0
							AND [Shown]=1
							AND [HasSchedual]=1
							AND @date BETWEEN S.[FromDate] AND S.[ToDate]
							AND RegionId = @regionId
							--		GROUP BY SI.[StartTime], SI.[EndTime]

						END
						ELSE
						BEGIN
							select  SI.*
							from scheduleitem  as SI , 
							Schedule as S,
							[dbo].[Resource] R,
							#TempResourceTypeID,
							[dbo].[ResourceTypes] RT
							where 
							S.[ResourceId]=R.[Id]
							and  SI.scheduleId= S.scheduleId
							AND #TempResourceTypeID.[ResourceTypeId]=RT.[Id]
							AND R.[ResourceTypeId]=RT.[Id]
							AND SI.[Day] =@date
						--	AND SI.[StartTime]= @startTime 
						--	AND SI.[EndTime]= @endTime
							and available= 1 
							and S.[IsDeleted]=0
							and SI.[IsDeleted]=0
							AND [Shown]=1
							AND [HasSchedual]=1
							AND @date BETWEEN S.[FromDate] AND S.[ToDate]
							--		GROUP BY SI.[StartTime], SI.[EndTime]

						END
				END --14 
			END --13
		END  ---2
    END try-----1
	begin catch 
			select 0
	end catch");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROC GetAvailableTimes");


        }
    }
}
