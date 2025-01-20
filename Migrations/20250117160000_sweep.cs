using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MYGUYY.Migrations
{
    /// <inheritdoc />
    public partial class sweep : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_TaskRequests_TaskRequestId1",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_TaskRequestId1",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "TaskRequestId1",
                table: "Messages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TaskRequestId1",
                table: "Messages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_TaskRequestId1",
                table: "Messages",
                column: "TaskRequestId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_TaskRequests_TaskRequestId1",
                table: "Messages",
                column: "TaskRequestId1",
                principalTable: "TaskRequests",
                principalColumn: "Id");
        }
    }
}
