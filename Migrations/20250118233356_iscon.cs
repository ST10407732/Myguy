using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MYGUYY.Migrations
{
    /// <inheritdoc />
    public partial class iscon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAgreementConfirmed",
                table: "TaskRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAgreementConfirmed",
                table: "TaskRequests");
        }
    }
}
