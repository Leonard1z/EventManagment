using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SetRegistrationToCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_TicketTypes_TicketTypeId",
                table: "Registrations");

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_TicketTypes_TicketTypeId",
                table: "Registrations",
                column: "TicketTypeId",
                principalTable: "TicketTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_TicketTypes_TicketTypeId",
                table: "Registrations");

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_TicketTypes_TicketTypeId",
                table: "Registrations",
                column: "TicketTypeId",
                principalTable: "TicketTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
