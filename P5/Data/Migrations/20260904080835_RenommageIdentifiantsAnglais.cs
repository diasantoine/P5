using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace P5.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenommageIdentifiantsAnglais : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Renommage pur : RenameTable/RenameColumn/RenameIndex conservent les
            // données existantes (contrairement au DropTable+CreateTable généré par
            // défaut par "dotnet ef migrations add", qui perdrait tout sans le seed).

            migrationBuilder.RenameTable(name: "Marques", newName: "Brands");
            migrationBuilder.RenameTable(name: "ModelesVoiture", newName: "CarModels");
            migrationBuilder.RenameTable(name: "Vehicules", newName: "Vehicles");
            migrationBuilder.RenameTable(name: "Reparations", newName: "Repairs");

            // L'index filtré sur CodeVin ne suit pas un renommage de colonne : SQL
            // Server ne met pas à jour le texte de son prédicat ("[CodeVin] IS NOT
            // NULL"). Il doit être supprimé avant le renommage de la colonne et
            // recréé ensuite avec le nouveau nom.
            migrationBuilder.DropIndex(name: "IX_Vehicules_CodeVin", table: "Vehicles");

            migrationBuilder.RenameColumn(name: "Nom", table: "Brands", newName: "Name");

            migrationBuilder.RenameColumn(name: "Nom", table: "CarModels", newName: "Name");
            migrationBuilder.RenameColumn(name: "MarqueId", table: "CarModels", newName: "BrandId");

            migrationBuilder.RenameColumn(name: "CodeVin", table: "Vehicles", newName: "Vin");
            migrationBuilder.RenameColumn(name: "Annee", table: "Vehicles", newName: "Year");
            migrationBuilder.RenameColumn(name: "ModeleVoitureId", table: "Vehicles", newName: "CarModelId");
            migrationBuilder.RenameColumn(name: "Finition", table: "Vehicles", newName: "Trim");
            migrationBuilder.RenameColumn(name: "DateAchat", table: "Vehicles", newName: "PurchaseDate");
            migrationBuilder.RenameColumn(name: "PrixAchat", table: "Vehicles", newName: "PurchasePrice");
            migrationBuilder.RenameColumn(name: "DateDisponibilite", table: "Vehicles", newName: "AvailabilityDate");
            migrationBuilder.RenameColumn(name: "DateVente", table: "Vehicles", newName: "SaleDate");

            migrationBuilder.RenameColumn(name: "Libelle", table: "Repairs", newName: "Description");
            migrationBuilder.RenameColumn(name: "Cout", table: "Repairs", newName: "Cost");
            migrationBuilder.RenameColumn(name: "VehiculeId", table: "Repairs", newName: "VehicleId");

            migrationBuilder.RenameIndex(name: "IX_Marques_Nom", table: "Brands", newName: "IX_Brands_Name");
            migrationBuilder.RenameIndex(name: "IX_ModelesVoiture_MarqueId_Nom", table: "CarModels", newName: "IX_CarModels_BrandId_Name");
            migrationBuilder.RenameIndex(name: "IX_Vehicules_ModeleVoitureId", table: "Vehicles", newName: "IX_Vehicles_CarModelId");
            migrationBuilder.RenameIndex(name: "IX_Reparations_VehiculeId", table: "Repairs", newName: "IX_Repairs_VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_Vin",
                table: "Vehicles",
                column: "Vin",
                unique: true,
                filter: "[Vin] IS NOT NULL");

            // Note : les noms de contraintes PK_/FK_ (ex. "PK_Marques",
            // "FK_ModelesVoiture_Marques_MarqueId") ne sont pas renommés ici. Ils
            // continuent de fonctionner tels quels ; seuls tables, colonnes et index
            // sont dans le périmètre de ce renommage.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_Vehicles_Vin", table: "Vehicles");

            migrationBuilder.RenameIndex(name: "IX_Brands_Name", table: "Brands", newName: "IX_Marques_Nom");
            migrationBuilder.RenameIndex(name: "IX_CarModels_BrandId_Name", table: "CarModels", newName: "IX_ModelesVoiture_MarqueId_Nom");
            migrationBuilder.RenameIndex(name: "IX_Vehicles_CarModelId", table: "Vehicles", newName: "IX_Vehicules_ModeleVoitureId");
            migrationBuilder.RenameIndex(name: "IX_Repairs_VehicleId", table: "Repairs", newName: "IX_Reparations_VehiculeId");

            migrationBuilder.RenameColumn(name: "Name", table: "Brands", newName: "Nom");

            migrationBuilder.RenameColumn(name: "Name", table: "CarModels", newName: "Nom");
            migrationBuilder.RenameColumn(name: "BrandId", table: "CarModels", newName: "MarqueId");

            migrationBuilder.RenameColumn(name: "Vin", table: "Vehicles", newName: "CodeVin");
            migrationBuilder.RenameColumn(name: "Year", table: "Vehicles", newName: "Annee");
            migrationBuilder.RenameColumn(name: "CarModelId", table: "Vehicles", newName: "ModeleVoitureId");
            migrationBuilder.RenameColumn(name: "Trim", table: "Vehicles", newName: "Finition");
            migrationBuilder.RenameColumn(name: "PurchaseDate", table: "Vehicles", newName: "DateAchat");
            migrationBuilder.RenameColumn(name: "PurchasePrice", table: "Vehicles", newName: "PrixAchat");
            migrationBuilder.RenameColumn(name: "AvailabilityDate", table: "Vehicles", newName: "DateDisponibilite");
            migrationBuilder.RenameColumn(name: "SaleDate", table: "Vehicles", newName: "DateVente");

            migrationBuilder.RenameColumn(name: "Description", table: "Repairs", newName: "Libelle");
            migrationBuilder.RenameColumn(name: "Cost", table: "Repairs", newName: "Cout");
            migrationBuilder.RenameColumn(name: "VehicleId", table: "Repairs", newName: "VehiculeId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicules_CodeVin",
                table: "Vehicles",
                column: "CodeVin",
                unique: true,
                filter: "[CodeVin] IS NOT NULL");

            migrationBuilder.RenameTable(name: "Brands", newName: "Marques");
            migrationBuilder.RenameTable(name: "CarModels", newName: "ModelesVoiture");
            migrationBuilder.RenameTable(name: "Vehicles", newName: "Vehicules");
            migrationBuilder.RenameTable(name: "Repairs", newName: "Reparations");
        }
    }
}
