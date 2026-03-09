using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataContext.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToPlayHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "PlayHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PlayHistories_UserId",
                table: "PlayHistories",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayHistories_Users_UserId",
                table: "PlayHistories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayHistories_Users_UserId",
                table: "PlayHistories");

            migrationBuilder.DropIndex(
                name: "IX_PlayHistories_UserId",
                table: "PlayHistories");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PlayHistories");
        }
    }
}
