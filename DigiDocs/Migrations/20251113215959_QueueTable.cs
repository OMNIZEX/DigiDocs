using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigiDocs.Migrations
{
    /// <inheritdoc />
    public partial class QueueTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PatientQueueId",
                table: "PatientData",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PatientQueues",
                columns: table => new
                {
                    PatientQueueId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientQueue", x => x.PatientQueueId);
                    table.ForeignKey(
                        name: "FK_PatientQueue_PatientData",
                        column: x => x.PatientId,
                        principalTable: "PatientData",
                        principalColumn: "PatientId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientQueues_PatientId",
                table: "PatientQueues",
                column: "PatientId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientQueues");

            migrationBuilder.DropColumn(
                name: "PatientQueueId",
                table: "PatientData");
        }
    }
}
