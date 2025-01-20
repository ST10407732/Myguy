using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MYGUYY.Migrations
{
    /// <inheritdoc />
    public partial class confutsion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stop_TaskRequests_TaskRequestId",
                table: "Stop");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Stop",
                table: "Stop");

            migrationBuilder.RenameTable(
                name: "Stop",
                newName: "Stops");

            migrationBuilder.RenameIndex(
                name: "IX_Stop_TaskRequestId",
                table: "Stops",
                newName: "IX_Stops_TaskRequestId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Stops",
                table: "Stops",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Stops_TaskRequests_TaskRequestId",
                table: "Stops",
                column: "TaskRequestId",
                principalTable: "TaskRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stops_TaskRequests_TaskRequestId",
                table: "Stops");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Stops",
                table: "Stops");

            migrationBuilder.RenameTable(
                name: "Stops",
                newName: "Stop");

            migrationBuilder.RenameIndex(
                name: "IX_Stops_TaskRequestId",
                table: "Stop",
                newName: "IX_Stop_TaskRequestId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Stop",
                table: "Stop",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Stop_TaskRequests_TaskRequestId",
                table: "Stop",
                column: "TaskRequestId",
                principalTable: "TaskRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
