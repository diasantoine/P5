using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace P5.Data.Migrations
{
    /// <inheritdoc />
    public partial class TableIntermediaireSpecification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Clés alternatives exigées par les clés étrangères composites.
            migrationBuilder.AddUniqueConstraint("AK_CarModels_Id_BrandId", "CarModels", new[] { "Id", "BrandId" });
            migrationBuilder.AddUniqueConstraint("AK_Trims_Id_CarModelId", "Trims", new[] { "Id", "CarModelId" });

            // 2. La table intermédiaire.
            migrationBuilder.CreateTable(
                name: "VehicleSpecifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandId = table.Column<int>(type: "int", nullable: false),
                    CarModelId = table.Column<int>(type: "int", nullable: false),
                    TrimId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleSpecifications", x => x.Id);
                    table.ForeignKey("FK_VehicleSpecifications_Brands_BrandId",
                        column: x => x.BrandId, principalTable: "Brands", principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_VehicleSpecifications_CarModels_CarModelId_BrandId",
                        columns: x => new { x.CarModelId, x.BrandId }, principalTable: "CarModels",
                        principalColumns: new[] { "Id", "BrandId" }, onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_VehicleSpecifications_Trims_TrimId_CarModelId",
                        columns: x => new { x.TrimId, x.CarModelId }, principalTable: "Trims",
                        principalColumns: new[] { "Id", "CarModelId" }, onDelete: ReferentialAction.Restrict);
                });

            // 3. Une spécification par finition existante. Une finition n'appartient qu'à un
            //    modèle, qui n'appartient qu'à une marque : le triplet est déterminé, et toutes
            //    les entrées du catalogue restent proposables dans la liste déroulante.
            migrationBuilder.Sql("""
                INSERT INTO [VehicleSpecifications] ([BrandId], [CarModelId], [TrimId])
                SELECT m.[BrandId], t.[CarModelId], t.[Id]
                FROM [Trims] t
                JOIN [CarModels] m ON m.[Id] = t.[CarModelId];
                """);

            migrationBuilder.CreateIndex("IX_VehicleSpecifications_BrandId_CarModelId_TrimId",
                "VehicleSpecifications", new[] { "BrandId", "CarModelId", "TrimId" }, unique: true);
            migrationBuilder.CreateIndex("IX_VehicleSpecifications_CarModelId_BrandId",
                "VehicleSpecifications", new[] { "CarModelId", "BrandId" });
            migrationBuilder.CreateIndex("IX_VehicleSpecifications_TrimId_CarModelId",
                "VehicleSpecifications", new[] { "TrimId", "CarModelId" });

            // 4. La nouvelle clé étrangère du véhicule, remplie depuis l'ancienne.
            migrationBuilder.AddColumn<int>("SpecificationId", "Vehicles", "int", nullable: false, defaultValue: 0);

            migrationBuilder.Sql("""
                UPDATE v
                SET v.[SpecificationId] = s.[Id]
                FROM [Vehicles] v
                JOIN [VehicleSpecifications] s ON s.[TrimId] = v.[TrimId];
                """);

            migrationBuilder.CreateIndex("IX_Vehicles_SpecificationId", "Vehicles", "SpecificationId");
            migrationBuilder.AddForeignKey("FK_Vehicles_VehicleSpecifications_SpecificationId",
                "Vehicles", "SpecificationId", "VehicleSpecifications", principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // 5. Seulement maintenant, le lien direct vers la finition disparaît.
            migrationBuilder.DropForeignKey("FK_Vehicles_Trims_TrimId", "Vehicles");
            migrationBuilder.DropIndex("IX_Vehicles_TrimId", "Vehicles");
            migrationBuilder.DropColumn("TrimId", "Vehicles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Miroir exact de Up() : on recrée d'abord la colonne directe vers la finition,
            // on la remplit depuis la spécification, puis on démonte la table intermédiaire.
            migrationBuilder.AddColumn<int>("TrimId", "Vehicles", "int", nullable: false, defaultValue: 0);

            migrationBuilder.Sql("""
                UPDATE v
                SET v.[TrimId] = s.[TrimId]
                FROM [Vehicles] v
                JOIN [VehicleSpecifications] s ON s.[Id] = v.[SpecificationId];
                """);

            migrationBuilder.CreateIndex("IX_Vehicles_TrimId", "Vehicles", "TrimId");
            migrationBuilder.AddForeignKey("FK_Vehicles_Trims_TrimId",
                "Vehicles", "TrimId", "Trims", principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.DropForeignKey("FK_Vehicles_VehicleSpecifications_SpecificationId", "Vehicles");
            migrationBuilder.DropIndex("IX_Vehicles_SpecificationId", "Vehicles");
            migrationBuilder.DropColumn("SpecificationId", "Vehicles");

            migrationBuilder.DropTable("VehicleSpecifications");

            migrationBuilder.DropUniqueConstraint("AK_Trims_Id_CarModelId", "Trims");
            migrationBuilder.DropUniqueConstraint("AK_CarModels_Id_BrandId", "CarModels");
        }
    }
}
