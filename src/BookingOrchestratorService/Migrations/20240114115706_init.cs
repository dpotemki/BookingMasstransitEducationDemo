using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingOrchestratorService.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookingStates",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentState = table.Column<string>(type: "text", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingStates", x => x.CorrelationId);
                });

            migrationBuilder.CreateTable(
                name: "ServiceStates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    RefusalReason = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceStates_BookingStates_BookingId",
                        column: x => x.BookingId,
                        principalTable: "BookingStates",
                        principalColumn: "CorrelationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingStates_CorrelationId",
                table: "BookingStates",
                column: "CorrelationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceStates_BookingId",
                table: "ServiceStates",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceStates_Id",
                table: "ServiceStates",
                column: "Id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceStates");

            migrationBuilder.DropTable(
                name: "BookingStates");
        }
    }
}
