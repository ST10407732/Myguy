using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MYGUYY.Migrations
{
    /// <inheritdoc />
    public partial class track : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DriverLatitude",
                table: "TaskRequests",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DriverLongitude",
                table: "TaskRequests",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DriverLatitude",
                table: "TaskRequests");

            migrationBuilder.DropColumn(
                name: "DriverLongitude",
                table: "TaskRequests");
        }
    }
}
