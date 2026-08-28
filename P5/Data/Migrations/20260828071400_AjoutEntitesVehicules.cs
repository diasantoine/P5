using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace P5.Data.Migrations
{
    /// <inheritdoc />
    public partial class AjoutEntitesVehicules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Marques",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marques", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModelesVoiture",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MarqueId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelesVoiture", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModelesVoiture_Marques_MarqueId",
                        column: x => x.MarqueId,
                        principalTable: "Marques",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vehicules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodeVin = table.Column<string>(type: "nvarchar(17)", maxLength: 17, nullable: true),
                    Annee = table.Column<int>(type: "int", nullable: false),
                    ModeleVoitureId = table.Column<int>(type: "int", nullable: false),
                    Finition = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    DateAchat = table.Column<DateOnly>(type: "date", nullable: false),
                    PrixAchat = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    DateDisponibilite = table.Column<DateOnly>(type: "date", nullable: true),
                    DateVente = table.Column<DateOnly>(type: "date", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PhotoUrl = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehicules_ModelesVoiture_ModeleVoitureId",
                        column: x => x.ModeleVoitureId,
                        principalTable: "ModelesVoiture",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reparations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Libelle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Cout = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    VehiculeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reparations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reparations_Vehicules_VehiculeId",
                        column: x => x.VehiculeId,
                        principalTable: "Vehicules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Marques_Nom",
                table: "Marques",
                column: "Nom",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModelesVoiture_MarqueId_Nom",
                table: "ModelesVoiture",
                columns: new[] { "MarqueId", "Nom" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reparations_VehiculeId",
                table: "Reparations",
                column: "VehiculeId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicules_CodeVin",
                table: "Vehicules",
                column: "CodeVin",
                unique: true,
                filter: "[CodeVin] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicules_ModeleVoitureId",
                table: "Vehicules",
                column: "ModeleVoitureId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reparations");

            migrationBuilder.DropTable(
                name: "Vehicules");

            migrationBuilder.DropTable(
                name: "ModelesVoiture");

            migrationBuilder.DropTable(
                name: "Marques");
        }
    }
}
