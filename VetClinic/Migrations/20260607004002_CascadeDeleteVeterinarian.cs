using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VetClinic.Migrations
{
    /// <inheritdoc />
    public partial class CascadeDeleteVeterinarian : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Veterinarians_VeterinarianId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Treatments_Veterinarians_VeterinarianId",
                table: "Treatments");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Veterinarians_VeterinarianId",
                table: "Appointments",
                column: "VeterinarianId",
                principalTable: "Veterinarians",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Treatments_Veterinarians_VeterinarianId",
                table: "Treatments",
                column: "VeterinarianId",
                principalTable: "Veterinarians",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Veterinarians_VeterinarianId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Treatments_Veterinarians_VeterinarianId",
                table: "Treatments");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Veterinarians_VeterinarianId",
                table: "Appointments",
                column: "VeterinarianId",
                principalTable: "Veterinarians",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Treatments_Veterinarians_VeterinarianId",
                table: "Treatments",
                column: "VeterinarianId",
                principalTable: "Veterinarians",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
