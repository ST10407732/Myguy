using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MYGUYY.Migrations
{
    /// <inheritdoc />
    public partial class removemulti : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskStop");

            migrationBuilder.DropColumn(
                name: "EstimatedDistance",
                table: "TaskRequests");

            migrationBuilder.DropColumn(
                name: "EstimatedDuration",
                table: "TaskRequests");

            migrationBuilder.DropColumn(
                name: "VehicleType",
                table: "TaskRequests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "EstimatedDistance",
                table: "TaskRequests",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "EstimatedDuration",
                table: "TaskRequests",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VehicleType",
                table: "TaskRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "TaskStop",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskRequestId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    StopOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskStop", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskStop_TaskRequests_TaskRequestId",
                        column: x => x.TaskRequestId,
                        principalTable: "TaskRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskStop_TaskRequestId",
                table: "TaskStop",
                column: "TaskRequestId");
        }
    }
}
