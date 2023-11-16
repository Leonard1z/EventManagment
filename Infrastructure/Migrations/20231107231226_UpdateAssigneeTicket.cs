using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAssigneeTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EventName",
                table: "AssignetTickets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "EventStartDate",
                table: "AssignetTickets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<double>(
                name: "TicketPrice",
                table: "AssignetTickets",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "TicketType",
                table: "AssignetTickets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Venue",
                table: "AssignetTickets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventName",
                table: "AssignetTickets");

            migrationBuilder.DropColumn(
                name: "EventStartDate",
                table: "AssignetTickets");

            migrationBuilder.DropColumn(
                name: "TicketPrice",
                table: "AssignetTickets");

            migrationBuilder.DropColumn(
                name: "TicketType",
                table: "AssignetTickets");

            migrationBuilder.DropColumn(
                name: "Venue",
                table: "AssignetTickets");
        }
    }
}
