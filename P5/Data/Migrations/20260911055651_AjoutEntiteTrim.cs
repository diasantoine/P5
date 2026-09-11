using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace P5.Data.Migrations
{
    /// <summary>
    /// La finition devient une entité du catalogue (Marque > Modèle > Finition) et le
    /// véhicule ne référence plus que sa finition : Vehicle -> Trim -> CarModel -> Brand.
    ///
    /// Écrite à la main. La version générée par "dotnet ef migrations add" faisait deux
    /// choses inacceptables : elle supprimait la colonne texte Trim AVANT d'en avoir
    /// tiré les lignes de la nouvelle table, et elle renommait CarModelId en TrimId en
    /// comptant sur le fait que les identifiants coïncident dans le seed — vrai pour les
    /// 7 véhicules de départ, faux pour tout véhicule saisi ensuite.
    /// Ici les données existantes sont converties avant que quoi que ce soit ne soit
    /// supprimé, quel que soit le contenu de la base.
    /// </summary>
    public partial class AjoutEntiteTrim : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. La table des finitions, rattachée aux modèles.
            migrationBuilder.CreateTable(
                name: "Trims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CarModelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trims_CarModels_CarModelId",
                        column: x => x.CarModelId,
                        principalTable: "CarModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // 2. Les 7 finitions de l'inventaire de départ, avec les identifiants que
            //    le snapshot du modèle leur connaît (même Id que leur modèle).
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

            // 3. Toute finition saisie en texte libre sur un véhicule et encore inconnue
            //    devient une ligne de Trims. Un véhicule sans finition reçoit une
            //    finition « Non précisée » pour son modèle : la chaîne exige une finition.
            migrationBuilder.Sql("""
                INSERT INTO [Trims] ([Name], [CarModelId])
                SELECT DISTINCT ISNULL(v.[Trim], N'Non précisée'), v.[CarModelId]
                FROM [Vehicles] v
                WHERE NOT EXISTS (
                    SELECT 1 FROM [Trims] t
                    WHERE t.[CarModelId] = v.[CarModelId]
                      AND t.[Name] = ISNULL(v.[Trim], N'Non précisée'));
                """);

            // 4. La nouvelle clé étrangère, remplie à partir de l'ancien couple
            //    (CarModelId, Trim) avant de recevoir sa contrainte.
            migrationBuilder.AddColumn<int>(
                name: "TrimId",
                table: "Vehicles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("""
                UPDATE v
                SET v.[TrimId] = t.[Id]
                FROM [Vehicles] v
                JOIN [Trims] t ON t.[CarModelId] = v.[CarModelId]
                              AND t.[Name] = ISNULL(v.[Trim], N'Non précisée');
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Trims_CarModelId_Name",
                table: "Trims",
                columns: new[] { "CarModelId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_TrimId",
                table: "Vehicles",
                column: "TrimId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Trims_TrimId",
                table: "Vehicles",
                column: "TrimId",
                principalTable: "Trims",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // 5. Seulement maintenant, l'ancien rattachement direct au modèle disparaît.
            //    La clé étrangère porte encore son nom français : la migration de
            //    renommage n'avait touché ni aux PK ni aux FK.
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicules_ModelesVoiture_ModeleVoitureId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_CarModelId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "CarModelId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "Trim",
                table: "Vehicles");

            // 6. Au passage, les contraintes restées en français prennent leur nom
            //    anglais : c'est ce décalage qui aurait fait échouer la version générée
            //    de cette migration, et il aurait piégé chaque migration suivante.
            migrationBuilder.Sql("EXEC sp_rename N'PK_Marques', N'PK_Brands';");
            migrationBuilder.Sql("EXEC sp_rename N'PK_ModelesVoiture', N'PK_CarModels';");
            migrationBuilder.Sql("EXEC sp_rename N'PK_Vehicules', N'PK_Vehicles';");
            migrationBuilder.Sql("EXEC sp_rename N'PK_Reparations', N'PK_Repairs';");
            migrationBuilder.Sql("EXEC sp_rename N'FK_ModelesVoiture_Marques_MarqueId', N'FK_CarModels_Brands_BrandId';");
            migrationBuilder.Sql("EXEC sp_rename N'FK_Reparations_Vehicules_VehiculeId', N'FK_Repairs_Vehicles_VehicleId';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("EXEC sp_rename N'FK_Repairs_Vehicles_VehicleId', N'FK_Reparations_Vehicules_VehiculeId';");
            migrationBuilder.Sql("EXEC sp_rename N'FK_CarModels_Brands_BrandId', N'FK_ModelesVoiture_Marques_MarqueId';");
            migrationBuilder.Sql("EXEC sp_rename N'PK_Repairs', N'PK_Reparations';");
            migrationBuilder.Sql("EXEC sp_rename N'PK_Vehicles', N'PK_Vehicules';");
            migrationBuilder.Sql("EXEC sp_rename N'PK_CarModels', N'PK_ModelesVoiture';");
            migrationBuilder.Sql("EXEC sp_rename N'PK_Brands', N'PK_Marques';");

            // Le rattachement direct au modèle et la finition en texte libre reviennent,
            // recalculés depuis la chaîne Vehicle -> Trim avant que la table ne disparaisse.
            migrationBuilder.AddColumn<int>(
                name: "CarModelId",
                table: "Vehicles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Trim",
                table: "Vehicles",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE v
                SET v.[CarModelId] = t.[CarModelId],
                    v.[Trim] = CASE WHEN t.[Name] = N'Non précisée' THEN NULL ELSE t.[Name] END
                FROM [Vehicles] v
                JOIN [Trims] t ON t.[Id] = v.[TrimId];
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_CarModelId",
                table: "Vehicles",
                column: "CarModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicules_ModelesVoiture_ModeleVoitureId",
                table: "Vehicles",
                column: "CarModelId",
                principalTable: "CarModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Trims_TrimId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_TrimId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "TrimId",
                table: "Vehicles");

            migrationBuilder.DropTable(
                name: "Trims");
        }
    }
}
