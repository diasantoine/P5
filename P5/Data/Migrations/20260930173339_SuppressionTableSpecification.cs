using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace P5.Data.Migrations
{
    /// <summary>
    /// Le véhicule référence directement sa finition : Vehicle -> Trim -> CarModel -> Brand.
    /// La table de spécification est supprimée après la copie de la finition de chaque véhicule.
    /// </summary>
    public partial class SuppressionTableSpecification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. La nouvelle colonne, remplie depuis la spécification de chaque véhicule.
            migrationBuilder.AddColumn<int>(
                name: "TrimId",
                table: "Vehicles",
                type: "int",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE v SET v.TrimId = s.TrimId
                FROM Vehicles v
                INNER JOIN VehicleSpecifications s ON s.Id = v.SpecificationId;
                """);

            migrationBuilder.AlterColumn<int>(
                name: "TrimId",
                table: "Vehicles",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            // 2. L'ancien lien vers la spécification disparaît.
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleSpecifications_SpecificationId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_SpecificationId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "SpecificationId",
                table: "Vehicles");

            // 3. La clé étrangère vers la finition.
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

            // 4. La table de spécification, puis les clés alternatives qui ne servaient qu'à elle.
            migrationBuilder.DropTable(
                name: "VehicleSpecifications");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Trims_Id_CarModelId",
                table: "Trims");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_CarModels_Id_BrandId",
                table: "CarModels");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Miroir de Up() : la table est recréée avec une spécification par finition,
            // puis chaque véhicule retrouve la spécification de sa finition.
            migrationBuilder.AddUniqueConstraint(
                name: "AK_CarModels_Id_BrandId",
                table: "CarModels",
                columns: new[] { "Id", "BrandId" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Trims_Id_CarModelId",
                table: "Trims",
                columns: new[] { "Id", "CarModelId" });

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
                    table.ForeignKey(
                        name: "FK_VehicleSpecifications_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleSpecifications_CarModels_CarModelId_BrandId",
                        columns: x => new { x.CarModelId, x.BrandId },
                        principalTable: "CarModels",
                        principalColumns: new[] { "Id", "BrandId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleSpecifications_Trims_TrimId_CarModelId",
                        columns: x => new { x.TrimId, x.CarModelId },
                        principalTable: "Trims",
                        principalColumns: new[] { "Id", "CarModelId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleSpecifications_BrandId_CarModelId_TrimId",
                table: "VehicleSpecifications",
                columns: new[] { "BrandId", "CarModelId", "TrimId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleSpecifications_CarModelId_BrandId",
                table: "VehicleSpecifications",
                columns: new[] { "CarModelId", "BrandId" });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleSpecifications_TrimId_CarModelId",
                table: "VehicleSpecifications",
                columns: new[] { "TrimId", "CarModelId" });

            migrationBuilder.Sql("""
                INSERT INTO VehicleSpecifications (BrandId, CarModelId, TrimId)
                SELECT m.BrandId, t.CarModelId, t.Id
                FROM Trims t
                INNER JOIN CarModels m ON m.Id = t.CarModelId;
                """);

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Trims_TrimId",
                table: "Vehicles");

            migrationBuilder.AddColumn<int>(
                name: "SpecificationId",
                table: "Vehicles",
                type: "int",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE v SET v.SpecificationId = s.Id
                FROM Vehicles v
                INNER JOIN VehicleSpecifications s ON s.TrimId = v.TrimId;
                """);

            migrationBuilder.AlterColumn<int>(
                name: "SpecificationId",
                table: "Vehicles",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_TrimId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "TrimId",
                table: "Vehicles");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_SpecificationId",
                table: "Vehicles",
                column: "SpecificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleSpecifications_SpecificationId",
                table: "Vehicles",
                column: "SpecificationId",
                principalTable: "VehicleSpecifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
