using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigiDocs.Migrations
{
    /// <inheritdoc />
    public partial class editBuggedField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PServiceType",
                table: "PatientData",
                newName: "PserviceType");

            migrationBuilder.RenameColumn(
                name: "PPriority",
                table: "PatientData",
                newName: "Ppriority");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PserviceType",
                table: "PatientData",
                newName: "PServiceType");

            migrationBuilder.RenameColumn(
                name: "Ppriority",
                table: "PatientData",
                newName: "PPriority");
        }
    }
}
