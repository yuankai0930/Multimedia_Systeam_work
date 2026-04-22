using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCompany.MyApp.Migrations
{
    /// <inheritdoc />
    public partial class AddApodMediaType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MediaType",
                table: "AppApodImages",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MediaType",
                table: "AppApodImages");
        }
    }
}
