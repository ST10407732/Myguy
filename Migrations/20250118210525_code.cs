using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MYGUYY.Migrations
{
    /// <inheritdoc />
    public partial class code : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgreementCode",
                table: "TaskRequests");

            migrationBuilder.DropColumn(
                name: "IsAgreementConfirmed",
                table: "TaskRequests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AgreementCode",
                table: "TaskRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsAgreementConfirmed",
                table: "TaskRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
