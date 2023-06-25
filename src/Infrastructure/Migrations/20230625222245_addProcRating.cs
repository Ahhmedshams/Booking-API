using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addProcRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"Create PROCEDURE SetRating
                                @ResourceID INT
                            WITH ENCRYPTION 
                            AS
                            BEGIN
                                DECLARE @AverageRating DECIMAL(10, 2)

                                SELECT @AverageRating = AVG(CAST([Rating] AS DECIMAL(18, 2)))
                                FROM [dbo].[ResourceReview]
                                WHERE [ResourceId] = @ResourceID

	                            update [dbo].[Resource]
                                set [Rating]=@AverageRating
	                            where [Id]=@ResourceID
                            END ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP SetRating");

        }
    }
}
