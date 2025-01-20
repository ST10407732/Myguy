using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MYGUYY.Migrations
{
    /// <inheritdoc />
    public partial class comebacka : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "TaskRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "TaskRequests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskRequests_UserId",
                table: "TaskRequests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskRequests_UserId1",
                table: "TaskRequests",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskRequests_Users_UserId",
                table: "TaskRequests",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskRequests_Users_UserId1",
                table: "TaskRequests",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskRequests_Users_UserId",
                table: "TaskRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskRequests_Users_UserId1",
                table: "TaskRequests");

            migrationBuilder.DropIndex(
                name: "IX_TaskRequests_UserId",
                table: "TaskRequests");

            migrationBuilder.DropIndex(
                name: "IX_TaskRequests_UserId1",
                table: "TaskRequests");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TaskRequests");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "TaskRequests");
        }
    }
}
