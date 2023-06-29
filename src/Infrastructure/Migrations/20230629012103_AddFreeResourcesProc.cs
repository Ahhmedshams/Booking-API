using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFreeResourcesProc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"create proc FreeResourcesOfBooking @BookingID int, @bookingStatus Varchar(50) 
with encryption 
as
	begin try 
	declare @time time 
	declare @date date
	declare @serviceID int


 CREATE TABLE #TempResourceTypeIDs (
        ResourceTypeId INT,
        NoOfResourcesRequired INT
    );

CREATE TABLE #TempALLRESOURCES (
        ResourceId INT,
    );
CREATE TABLE #TempResourceWithScheduleVisible (
        ResourceId INT,
    );
CREATE TABLE #TempResourceWithScheduleInvisible (
        ResourceId INT,
    );
CREATE TABLE #TempResourceNoScheduleInvisible (
        ResourceId INT,
    );

		IF EXISTS
		(
			select [Id], [StartTime]
			from [dbo].[ClientBookings]
			where [Id]=@BookingID
		)
		begin 
	
			select  @time=[StartTime],
			@date=[Date],
			@serviceID =[ServiceId]
			from [dbo].[ClientBookings]
			where [Id]=@BookingID

			INSERT INTO #TempResourceTypeIDs (ResourceTypeId, NoOfResourcesRequired)
			SELECT [ResourceTypeId], NoOfResources
			FROM [dbo].[ServiceMetadata]
			WHERE [ServiceId] = @serviceID;

			INSERT INTO #TempALLRESOURCES
			SELECT BI.[ResourceId] 
			FROM [dbo].[BookingItems] BI
			WHERE BI.[BookingId]=@BookingID
	

			insert into #TempResourceWithScheduleVisible 
			select R.[Id]
			from [dbo].[Resource] R,#TempResourceTypeIDs ,#TempALLRESOURCES,[dbo].[ResourceTypes] RT
			where R.[ResourceTypeId]=#TempResourceTypeIDs.ResourceTypeId
			and R.[Id]=#TempALLRESOURCES.ResourceId
			and RT.[Id]=#TempResourceTypeIDs.ResourceTypeId 
			and RT.[Shown]=1
			and RT.[HasSchedual]=1

			insert into #TempResourceWithScheduleInvisible 
			select R.[Id]
			from [dbo].[Resource] R,#TempResourceTypeIDs ,#TempALLRESOURCES,[dbo].[ResourceTypes] RT
			where R.[ResourceTypeId]=#TempResourceTypeIDs.ResourceTypeId
			and R.[Id]=#TempALLRESOURCES.ResourceId
			and RT.[Id]=#TempResourceTypeIDs.ResourceTypeId 
			and RT.[Shown]=0
			and RT.[HasSchedual]=1

			insert into #TempResourceNoScheduleInvisible 
			select R.[Id]
			from [dbo].[Resource] R,#TempResourceTypeIDs ,#TempALLRESOURCES,[dbo].[ResourceTypes] RT
			where R.[ResourceTypeId]=#TempResourceTypeIDs.ResourceTypeId
			and R.[Id]=#TempALLRESOURCES.ResourceId
			and RT.[Id]=#TempResourceTypeIDs.ResourceTypeId 
			and RT.[Shown]=0
			and RT.[HasSchedual]=0

		begin try 
			IF EXISTS
	     	(
				SELECT COUNT(*) FROM #TempResourceWithScheduleVisible
			)
			BEGIN 
				update  [dbo].[ScheduleItem] --1
				set [Available] =1 
				from [dbo].[ScheduleItem] as SI,[dbo].[Schedule]as S, #TempResourceWithScheduleVisible as TR
				where  S.[ScheduleId]=SI.[ScheduleID] 
				and S.[ResourceId]=TR.ResourceId 
				and SI.[Day]=@date 
				and SI.[StartTime]=@time
			END 

			IF EXISTS
	     	(
				SELECT COUNT(*) FROM #TempResourceWithScheduleInvisible
			)
			BEGIN TRY
				
				BEGIN TRANSACTION 
				 
					update  [dbo].[ScheduleItem] --2-1
					set [Available] =1 
					from [dbo].[ScheduleItem] as SI,[dbo].[Schedule]as S, #TempResourceWithScheduleInvisible as TR
					where  S.[ScheduleId]=SI.[ScheduleID] 
					and S.[ResourceId]=TR.ResourceId 
					and SI.[Day]=@date 
					and SI.[StartTime]=@time 

					update [dbo].[ResourceSpecialCharacteristics] --2-2
					set [AvailableCapacity]= [AvailableCapacity]+ RTids.NoOfResourcesRequired 
					from #TempResourceTypeIDs RTids ,[dbo].[ResourceTypes] RT,#TempResourceWithScheduleInvisible,
					[dbo].[ResourceSpecialCharacteristics] RSC,[dbo].[ScheduleItem] SI
					where  RTids.ResourceTypeId = RT.[Id] 
					AND #TempResourceWithScheduleInvisible.ResourceID=RSC.[ResourceID]
					AND RSC.[ScheduleID]=SI.[ID]
					AND  RT.[HasSchedual]=1
					AND RT.[Shown]=0
					commit;
				
			END TRY 
			BEGIN CATCH
						rollback;
			END CATCH 

			IF EXISTS
	     	(
				SELECT COUNT(*) FROM #TempResourceNoScheduleInvisible
			)
			BEGIN 
				update [dbo].[ResourceSpecialCharacteristics] --3
				set [AvailableCapacity]= [AvailableCapacity]+ RTids.NoOfResourcesRequired 
				from #TempResourceTypeIDs RTids ,[dbo].[ResourceTypes] RT,#TempResourceNoScheduleInvisible,
				[dbo].[ResourceSpecialCharacteristics] RSC
				where  RTids.ResourceTypeId = RT.[Id] 
				AND #TempResourceNoScheduleInvisible.ResourceID=RSC.[ResourceID]
				AND  RT.[HasSchedual]=0
				AND RT.[Shown]=0

			END 
		end try 
		begin catch
			select 0
		end catch

		begin try 
		BEGIN TRANSACTION;
			--update  [dbo].[BookingItems]
			--set [IsDeleted]=1
			--where [BookingId]=@BookingID

			update [dbo].[ClientBookings]
			set [Status]= @bookingStatus
			where [Id] =@BookingID 
			commit;
		end try
		
		begin catch 
			rollback;
			select 0
		end catch

		 
	end 
		else 
		select 0
	end try 
	begin catch 
		select 0
	end catch");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP Proc FreeResourcesOfBooking");

        }
    }
}
