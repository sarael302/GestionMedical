using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionMedical.Migrations.GestionMedical
{
    /// <inheritdoc />
    public partial class NotificationMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Medecin",
                columns: table => new
                {
                    MedecinId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Prenom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Specialite = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Disponible = table.Column<bool>(type: "bit", nullable: false),
                    DateRetour = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Medecin__69D27AFB3CD03740", x => x.MedecinId);
                });

            migrationBuilder.CreateTable(
                name: "Patient",
                columns: table => new
                {
                    PatientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Prenom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Adresse = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DateNaissance = table.Column<DateOnly>(type: "date", nullable: false),
                    Telephone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Patient__970EC366A7AAEC07", x => x.PatientId);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelMedical",
                columns: table => new
                {
                    PersonnelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Prenom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Fonction = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Personne__CAFBCB4FE8DEC8B9", x => x.PersonnelId);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DateSent = table.Column<DateTime>(type: "datetime", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    MedecinId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PKNotific4E3E04AD7882750B", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FKNotificMedec__51F18345",
                        column: x => x.MedecinId,
                        principalTable: "Medecin",
                        principalColumn: "MedecinId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RendezVous",
                columns: table => new
                {
                    RendezVousId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: true),
                    MedecinId = table.Column<int>(type: "int", nullable: true),
                    DateHeure = table.Column<DateTime>(type: "datetime", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Pending")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__RendezVo__C4B748C7D003D9D7", x => x.RendezVousId);
                    table.ForeignKey(
                        name: "FK__RendezVou__Medec__3F466844",
                        column: x => x.MedecinId,
                        principalTable: "Medecin",
                        principalColumn: "MedecinId");
                    table.ForeignKey(
                        name: "FK__RendezVou__Patie__3E52440B",
                        column: x => x.PatientId,
                        principalTable: "Patient",
                        principalColumn: "PatientId");
                });

            migrationBuilder.CreateTable(
                name: "Consultation",
                columns: table => new
                {
                    ConsultationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RendezVousId = table.Column<int>(type: "int", nullable: true),
                    Prix = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Resultat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ordonnance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrdreMedical = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Consulta__5D014A98BFD65BE3", x => x.ConsultationId);
                    table.ForeignKey(
                        name: "FK__Consultat__Rende__440B1D61",
                        column: x => x.RendezVousId,
                        principalTable: "RendezVous",
                        principalColumn: "RendezVousId");
                });

            migrationBuilder.CreateTable(
                name: "DossierMedical",
                columns: table => new
                {
                    DossierMedicalId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: true),
                    MedecinId = table.Column<int>(type: "int", nullable: true),
                    ConsultationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DossierM__19050E45D417C5A4", x => x.DossierMedicalId);
                    table.ForeignKey(
                        name: "FK__DossierMe__Consu__49C3F6B7",
                        column: x => x.ConsultationId,
                        principalTable: "Consultation",
                        principalColumn: "ConsultationId");
                    table.ForeignKey(
                        name: "FK__DossierMe__Medec__48CFD27E",
                        column: x => x.MedecinId,
                        principalTable: "Medecin",
                        principalColumn: "MedecinId");
                    table.ForeignKey(
                        name: "FK__DossierMe__Patie__47DBAE45",
                        column: x => x.PatientId,
                        principalTable: "Patient",
                        principalColumn: "PatientId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Consultation_RendezVousId",
                table: "Consultation",
                column: "RendezVousId");

            migrationBuilder.CreateIndex(
                name: "IX_DossierMedical_ConsultationId",
                table: "DossierMedical",
                column: "ConsultationId");

            migrationBuilder.CreateIndex(
                name: "IX_DossierMedical_MedecinId",
                table: "DossierMedical",
                column: "MedecinId");

            migrationBuilder.CreateIndex(
                name: "IX_DossierMedical_PatientId",
                table: "DossierMedical",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_MedecinId",
                table: "Notification",
                column: "MedecinId");

            migrationBuilder.CreateIndex(
                name: "IX_RendezVous_MedecinId",
                table: "RendezVous",
                column: "MedecinId");

            migrationBuilder.CreateIndex(
                name: "IX_RendezVous_PatientId",
                table: "RendezVous",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DossierMedical");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "PersonnelMedical");

            migrationBuilder.DropTable(
                name: "Consultation");

            migrationBuilder.DropTable(
                name: "RendezVous");

            migrationBuilder.DropTable(
                name: "Medecin");

            migrationBuilder.DropTable(
                name: "Patient");
        }
    }
}
