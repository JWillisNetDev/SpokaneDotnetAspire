using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpokaneDotnetAspire.Services.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUriToMeetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUri",
                table: "Meetups",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUri",
                table: "Meetups");
        }
    }
}
