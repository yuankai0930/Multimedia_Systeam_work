using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCompany.MyApp.Migrations
{
    /// <inheritdoc />
    public partial class AddApodQueryHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppApodQueryHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApodImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApodDate = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    QueryTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppApodQueryHistories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppApodImages_Date",
                table: "AppApodImages",
                column: "Date",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppApodQueryHistories_ApodImageId",
                table: "AppApodQueryHistories",
                column: "ApodImageId");

            migrationBuilder.CreateIndex(
                name: "IX_AppApodQueryHistories_UserId_QueryTime",
                table: "AppApodQueryHistories",
                columns: new[] { "UserId", "QueryTime" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppApodQueryHistories");

            migrationBuilder.DropIndex(
                name: "IX_AppApodImages_Date",
                table: "AppApodImages");
        }
    }
}
