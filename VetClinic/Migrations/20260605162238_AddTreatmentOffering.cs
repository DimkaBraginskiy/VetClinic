using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VetClinic.Migrations
{
    /// <inheritdoc />
    public partial class AddTreatmentOffering : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableTreatmentTypes",
                table: "Veterinarians");

            migrationBuilder.CreateTable(
                name: "TreatmentOfferings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    BaseDuration = table.Column<decimal>(type: "TEXT", nullable: false),
                    VeterinarianId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentOfferings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreatmentOfferings_Veterinarians_VeterinarianId",
                        column: x => x.VeterinarianId,
                        principalTable: "Veterinarians",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentOfferings_VeterinarianId",
                table: "TreatmentOfferings",
                column: "VeterinarianId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TreatmentOfferings");

            migrationBuilder.AddColumn<string>(
                name: "AvailableTreatmentTypes",
                table: "Veterinarians",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
