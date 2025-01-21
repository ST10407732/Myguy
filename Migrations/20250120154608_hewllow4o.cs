using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MYGUYY.Migrations
{
    /// <inheritdoc />
    public partial class hewllow4o : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DriverProfiles_Users_UserId",
                table: "DriverProfiles");

            migrationBuilder.DropIndex(
                name: "IX_DriverProfiles_UserId",
                table: "DriverProfiles");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "DriverProfiles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "DriverProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DriverProfiles_UserId",
                table: "DriverProfiles",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_DriverProfiles_Users_UserId",
                table: "DriverProfiles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
