using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace P5.Data.Migrations
{
    /// <summary>
    /// Retire les 7 lignes d'inventaire insérées par HasData ; SeedData.SeedAsync les
    /// réinsère au lancement si la base est vide.
    /// </summary>
    public partial class SeedApplicatif : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Repairs",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Repairs",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Repairs",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Repairs",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Repairs",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Repairs",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Repairs",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Trims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Trims",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Trims",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Trims",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Trims",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Trims",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Trims",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "CarModels",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CarModels",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CarModels",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "CarModels",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "CarModels",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "CarModels",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "CarModels",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 6);
            // Les compteurs d'identité repartent de zéro sur les tables vidées, pour que
            // le seed applicatif redonne les identifiants 1 à 7 de l'inventaire d'origine.
            // Une table qui contiendrait encore des lignes n'est pas touchée.
            foreach (var table in new[] { "Repairs", "Vehicles", "Trims", "CarModels", "Brands" })
            {
                migrationBuilder.Sql($"IF NOT EXISTS (SELECT 1 FROM [{table}]) DBCC CHECKIDENT ('[{table}]', RESEED, 0);");
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Brands",
                columns: new[] { "Id", "Name" },
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
                table: "CarModels",
                columns: new[] { "Id", "BrandId", "Name" },
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
                table: "Trims",
                columns: new[] { "Id", "CarModelId", "Name" },
                values: new object[,]
                {
                    { 1, 1, "LE" },
                    { 2, 2, "Sport" },
                    { 3, 3, "TCe" },
                    { 4, 4, "XLT" },
                    { 5, 5, "LX" },
                    { 6, 6, "S" },
                    { 7, 7, "SEL" }
                });

            migrationBuilder.InsertData(
                table: "Vehicles",
                columns: new[] { "Id", "AvailabilityDate", "Description", "PhotoUrl", "PurchaseDate", "PurchasePrice", "SaleDate", "TrimId", "Vin", "Year" },
                values: new object[,]
                {
                    { 1, new DateOnly(2022, 4, 7), null, null, new DateOnly(2022, 1, 7), 1800m, new DateOnly(2022, 4, 8), 1, null, 2019 },
                    { 2, new DateOnly(2022, 4, 7), null, null, new DateOnly(2022, 4, 2), 4500m, new DateOnly(2022, 4, 9), 2, null, 2007 },
                    { 3, new DateOnly(2022, 4, 8), null, null, new DateOnly(2022, 4, 4), 1800m, null, 3, null, 2007 },
                    { 4, new DateOnly(2022, 4, 9), null, null, new DateOnly(2022, 4, 5), 24350m, null, 4, null, 2017 },
                    { 5, new DateOnly(2022, 4, 9), null, null, new DateOnly(2022, 4, 6), 4000m, new DateOnly(2022, 4, 9), 5, null, 2008 },
                    { 6, new DateOnly(2022, 4, 10), null, null, new DateOnly(2022, 4, 6), 15250m, new DateOnly(2022, 4, 12), 6, null, 2016 },
                    { 7, new DateOnly(2022, 4, 11), null, null, new DateOnly(2022, 4, 7), 10990m, new DateOnly(2022, 4, 12), 7, null, 2013 }
                });

            migrationBuilder.InsertData(
                table: "Repairs",
                columns: new[] { "Id", "Cost", "Description", "VehicleId" },
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
    }
}
