using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class deleteTicketImagePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "TicketTypes");

            migrationBuilder.CreateIndex(
                name: "IX_AssignetTickets_RegistrationId",
                table: "AssignetTickets",
                column: "RegistrationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignetTickets_Registrations_RegistrationId",
                table: "AssignetTickets",
                column: "RegistrationId",
                principalTable: "Registrations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignetTickets_Registrations_RegistrationId",
                table: "AssignetTickets");

            migrationBuilder.DropIndex(
                name: "IX_AssignetTickets_RegistrationId",
                table: "AssignetTickets");

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "TicketTypes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
