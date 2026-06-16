using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCompany.MyApp.Migrations
{
    /// <inheritdoc />
    public partial class AddHistoryStarredAndPinnedOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsStarred",
                table: "AppApodQueryHistories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PinnedOrder",
                table: "AppApodQueryHistories",
                type: "int",
                nullable: true);

            // Drop old index
            migrationBuilder.DropIndex(
                name: "IX_AppApodQueryHistories_UserId_QueryTime",
                table: "AppApodQueryHistories");

            // Create new composite index
            migrationBuilder.CreateIndex(
                name: "IX_AppApodQueryHistories_UserId_IsStarred_QueryTime",
                table: "AppApodQueryHistories",
                columns: new[] { "UserId", "IsStarred", "QueryTime" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppApodQueryHistories_UserId_IsStarred_QueryTime",
                table: "AppApodQueryHistories");

            migrationBuilder.DropColumn(
                name: "IsStarred",
                table: "AppApodQueryHistories");

            migrationBuilder.DropColumn(
                name: "PinnedOrder",
                table: "AppApodQueryHistories");

            migrationBuilder.CreateIndex(
                name: "IX_AppApodQueryHistories_UserId_QueryTime",
                table: "AppApodQueryHistories",
                columns: new[] { "UserId", "QueryTime" });
        }
    }
}
