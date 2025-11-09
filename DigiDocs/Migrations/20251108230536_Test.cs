using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigiDocs.Migrations
{
    /// <inheritdoc />
    public partial class Test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Examinations",
                columns: table => new
                {
                    ExaminationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    LastModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    LastModifiedById = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Examinat__C4A9242084E14961", x => x.ExaminationId);
                });

            migrationBuilder.CreateTable(
                name: "Medicines",
                columns: table => new
                {
                    MedicineId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicineName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    LastModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    LastModifiedById = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Medicine__4F2128908F21F13A", x => x.MedicineId);
                });

            migrationBuilder.CreateTable(
                name: "SymptomsCategories",
                columns: table => new
                {
                    SymptomsCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SymptomsCategoryName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    LastModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    LastModifiedById = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Symptoms__4D23846DFFB73AE9", x => x.SymptomsCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "PatientData",
                columns: table => new
                {
                    PatientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PAge = table.Column<int>(type: "int", nullable: true),
                    PJob = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PGender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PComplain = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PChronicDisease = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PServiceType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PPriority = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PAmountToPay = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PPhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PNextAppointment = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExaminationId = table.Column<int>(type: "int", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    LastModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    LastModifiedById = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PatientD__970EC366E879BB66", x => x.PatientId);
                    table.ForeignKey(
                        name: "FK_PatientData_Examinations",
                        column: x => x.ExaminationId,
                        principalTable: "Examinations",
                        principalColumn: "ExaminationId");
                });

            migrationBuilder.CreateTable(
                name: "Symptoms",
                columns: table => new
                {
                    SymptomId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SymptomName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SymptomsCategoryId = table.Column<int>(type: "int", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    LastModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    LastModifiedById = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Symptoms__D26ED896AA7D1E06", x => x.SymptomId);
                    table.ForeignKey(
                        name: "FK_Symptoms_SymptomsCategories",
                        column: x => x.SymptomsCategoryId,
                        principalTable: "SymptomsCategories",
                        principalColumn: "SymptomsCategoryId");
                });

            migrationBuilder.CreateTable(
                name: "Diagnoses",
                columns: table => new
                {
                    DiagnosisId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientDiagnosis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientRegInvestigation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    ExaminationId = table.Column<int>(type: "int", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    LastModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    LastModifiedById = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Diagnose__0C54CC733224B3AA", x => x.DiagnosisId);
                    table.ForeignKey(
                        name: "FK_Diagnoses_Examinations",
                        column: x => x.ExaminationId,
                        principalTable: "Examinations",
                        principalColumn: "ExaminationId");
                    table.ForeignKey(
                        name: "FK_Diagnoses_PatientData",
                        column: x => x.PatientId,
                        principalTable: "PatientData",
                        principalColumn: "PatientId");
                });

            migrationBuilder.CreateTable(
                name: "PatientMedications",
                columns: table => new
                {
                    PatientMedId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    MedicineId = table.Column<int>(type: "int", nullable: false),
                    Dosage = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Frequency = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ExaminationId = table.Column<int>(type: "int", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    LastModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    LastModifiedById = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PatientM__47A48821B7EDC2A1", x => x.PatientMedId);
                    table.ForeignKey(
                        name: "FK_PatientMedications_Examinations",
                        column: x => x.ExaminationId,
                        principalTable: "Examinations",
                        principalColumn: "ExaminationId");
                    table.ForeignKey(
                        name: "FK_PatientMedications_Medicines",
                        column: x => x.MedicineId,
                        principalTable: "Medicines",
                        principalColumn: "MedicineId");
                    table.ForeignKey(
                        name: "FK_PatientMedications_PatientData",
                        column: x => x.PatientId,
                        principalTable: "PatientData",
                        principalColumn: "PatientId");
                });

            migrationBuilder.CreateTable(
                name: "PatientSymptoms",
                columns: table => new
                {
                    PatientSymId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SymptomId = table.Column<int>(type: "int", nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    ExaminationId = table.Column<int>(type: "int", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    LastModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    LastModifiedById = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PatientS__3A9CC5EDAA737287", x => x.PatientSymId);
                    table.ForeignKey(
                        name: "FK_PatientSymptoms_Examinations",
                        column: x => x.ExaminationId,
                        principalTable: "Examinations",
                        principalColumn: "ExaminationId");
                    table.ForeignKey(
                        name: "FK_PatientSymptoms_PatientData",
                        column: x => x.PatientId,
                        principalTable: "PatientData",
                        principalColumn: "PatientId");
                    table.ForeignKey(
                        name: "FK_PatientSymptoms_Symptoms",
                        column: x => x.SymptomId,
                        principalTable: "Symptoms",
                        principalColumn: "SymptomId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Diagnoses_ExaminationId",
                table: "Diagnoses",
                column: "ExaminationId");

            migrationBuilder.CreateIndex(
                name: "IX_Diagnoses_PatientId",
                table: "Diagnoses",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientData_ExaminationId",
                table: "PatientData",
                column: "ExaminationId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientMedications_ExaminationId",
                table: "PatientMedications",
                column: "ExaminationId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientMedications_MedicineId",
                table: "PatientMedications",
                column: "MedicineId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientMedications_PatientId",
                table: "PatientMedications",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientSymptoms_ExaminationId",
                table: "PatientSymptoms",
                column: "ExaminationId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientSymptoms_PatientId",
                table: "PatientSymptoms",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientSymptoms_SymptomId",
                table: "PatientSymptoms",
                column: "SymptomId");

            migrationBuilder.CreateIndex(
                name: "IX_Symptoms_SymptomsCategoryId",
                table: "Symptoms",
                column: "SymptomsCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Diagnoses");

            migrationBuilder.DropTable(
                name: "PatientMedications");

            migrationBuilder.DropTable(
                name: "PatientSymptoms");

            migrationBuilder.DropTable(
                name: "Medicines");

            migrationBuilder.DropTable(
                name: "PatientData");

            migrationBuilder.DropTable(
                name: "Symptoms");

            migrationBuilder.DropTable(
                name: "Examinations");

            migrationBuilder.DropTable(
                name: "SymptomsCategories");
        }
    }
}
