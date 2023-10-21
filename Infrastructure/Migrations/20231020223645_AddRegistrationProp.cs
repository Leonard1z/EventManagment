using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRegistrationProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Registrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "TicketPrice",
                table: "Registrations",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "TicketTypeId",
                table: "Registrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "TotalPrice",
                table: "Registrations",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "TransactionId",
                table: "Registrations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_TicketTypeId",
                table: "Registrations",
                column: "TicketTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_TicketTypes_TicketTypeId",
                table: "Registrations",
                column: "TicketTypeId",
                principalTable: "TicketTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_TicketTypes_TicketTypeId",
                table: "Registrations");

            migrationBuilder.DropIndex(
                name: "IX_Registrations_TicketTypeId",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "TicketPrice",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "TicketTypeId",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "Registrations");
        }
    }
}
