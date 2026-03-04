using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataContext.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayHistories_Users_UserId",
                table: "PlayHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Songs_Users_UserId",
                table: "Songs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlaylistSongs",
                table: "PlaylistSongs");

            migrationBuilder.DropIndex(
                name: "IX_PlayHistories_UserId",
                table: "PlayHistories");

            migrationBuilder.DropColumn(
                name: "EHoliday",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PlayHistories");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Songs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlaylistSongId",
                table: "PlaylistSongs",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlaylistSongs",
                table: "PlaylistSongs",
                column: "PlaylistSongId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistSongs_PlaylistId",
                table: "PlaylistSongs",
                column: "PlaylistId");

            migrationBuilder.AddForeignKey(
                name: "FK_Songs_Users_UserId",
                table: "Songs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Songs_Users_UserId",
                table: "Songs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlaylistSongs",
                table: "PlaylistSongs");

            migrationBuilder.DropIndex(
                name: "IX_PlaylistSongs_PlaylistId",
                table: "PlaylistSongs");

            migrationBuilder.DropColumn(
                name: "PlaylistSongId",
                table: "PlaylistSongs");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Songs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "EHoliday",
                table: "Songs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "PlayHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlaylistSongs",
                table: "PlaylistSongs",
                columns: new[] { "PlaylistId", "SongId" });

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

            migrationBuilder.AddForeignKey(
                name: "FK_Songs_Users_UserId",
                table: "Songs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
