using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace P5.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedInventaireInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Marques",
                columns: new[] { "Id", "Nom" },
                values: new object[,]
                {
                    { 1, "Mazda" },
                    { 2, "Jeep" },
                    { 3, "Renault" },
                    { 4, "Ford" },
                    { 5, "Honda" },
                    { 6, "Volkswagen" }
                });

            migrationBuilder.InsertData(
                table: "ModelesVoiture",
                columns: new[] { "Id", "MarqueId", "Nom" },
                values: new object[,]
                {
                    { 1, 1, "Miata" },
                    { 2, 2, "Liberty" },
                    { 3, 3, "Scénic" },
                    { 4, 4, "Explorer" },
                    { 5, 5, "Civic" },
                    { 6, 6, "GTI" },
                    { 7, 4, "Edge" }
                });

            migrationBuilder.InsertData(
                table: "Vehicules",
                columns: new[] { "Id", "Annee", "CodeVin", "DateAchat", "DateDisponibilite", "DateVente", "Description", "Finition", "ModeleVoitureId", "PhotoUrl", "PrixAchat" },
                values: new object[,]
                {
                    { 1, 2019, null, new DateOnly(2022, 1, 7), new DateOnly(2022, 4, 7), new DateOnly(2022, 4, 8), null, "LE", 1, null, 1800m },
                    { 2, 2007, null, new DateOnly(2022, 4, 2), new DateOnly(2022, 4, 7), new DateOnly(2022, 4, 9), null, "Sport", 2, null, 4500m },
                    { 3, 2007, null, new DateOnly(2022, 4, 4), new DateOnly(2022, 4, 8), null, null, "TCe", 3, null, 1800m },
                    { 4, 2017, null, new DateOnly(2022, 4, 5), new DateOnly(2022, 4, 9), null, null, "XLT", 4, null, 24350m },
                    { 5, 2008, null, new DateOnly(2022, 4, 6), new DateOnly(2022, 4, 9), new DateOnly(2022, 4, 9), null, "LX", 5, null, 4000m },
                    { 6, 2016, null, new DateOnly(2022, 4, 6), new DateOnly(2022, 4, 10), new DateOnly(2022, 4, 12), null, "S", 6, null, 15250m },
                    { 7, 2013, null, new DateOnly(2022, 4, 7), new DateOnly(2022, 4, 11), new DateOnly(2022, 4, 12), null, "SEL", 7, null, 10990m }
                });

            migrationBuilder.InsertData(
                table: "Reparations",
                columns: new[] { "Id", "Cout", "Libelle", "VehiculeId" },
                values: new object[,]
                {
                    { 1, 7600m, "Restauration complète", 1 },
                    { 2, 350m, "Roulements des roues avant", 2 },
                    { 3, 690m, "Radiateur, freins", 3 },
                    { 4, 1100m, "Pneus, freins", 4 },
                    { 5, 475m, "Climatisation, freins", 5 },
                    { 6, 440m, "Pneus", 6 },
                    { 7, 950m, "Pneus, freins, climatisation", 7 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Reparations",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Reparations",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Reparations",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Reparations",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Reparations",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Reparations",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Reparations",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Vehicules",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Vehicules",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Vehicules",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Vehicules",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Vehicules",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Vehicules",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Vehicules",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ModelesVoiture",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ModelesVoiture",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ModelesVoiture",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ModelesVoiture",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ModelesVoiture",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ModelesVoiture",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ModelesVoiture",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Marques",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Marques",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Marques",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Marques",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Marques",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Marques",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}
