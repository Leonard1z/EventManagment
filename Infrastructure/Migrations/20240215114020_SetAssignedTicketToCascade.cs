using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SetAssignedTicketToCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignetTickets_Registrations_RegistrationId",
                table: "AssignetTickets");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignetTickets_Registrations_RegistrationId",
                table: "AssignetTickets",
                column: "RegistrationId",
                principalTable: "Registrations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignetTickets_Registrations_RegistrationId",
                table: "AssignetTickets");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignetTickets_Registrations_RegistrationId",
                table: "AssignetTickets",
                column: "RegistrationId",
                principalTable: "Registrations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
