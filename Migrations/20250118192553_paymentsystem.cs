using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MYGUYY.Migrations
{
    /// <inheritdoc />
    public partial class paymentsystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AcceptedAt",
                table: "TaskRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AgreementCode",
                table: "TaskRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ClientSignature",
                table: "TaskRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ConfirmedAt",
                table: "TaskRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DriverSignature",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptedAt",
                table: "TaskRequests");

            migrationBuilder.DropColumn(
                name: "AgreementCode",
                table: "TaskRequests");

            migrationBuilder.DropColumn(
                name: "ClientSignature",
                table: "TaskRequests");

            migrationBuilder.DropColumn(
                name: "ConfirmedAt",
                table: "TaskRequests");

            migrationBuilder.DropColumn(
                name: "DriverSignature",
                table: "TaskRequests");

            migrationBuilder.DropColumn(
                name: "IsAgreementConfirmed",
                table: "TaskRequests");
        }
    }
}
