using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using P5.Models;

namespace P5.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        // Un DbSet = une table. Sans DbSet, pas de table générée.
        public DbSet<Brand> Brands => Set<Brand>();
        public DbSet<CarModel> CarModels => Set<CarModel>();
        public DbSet<Vehicle> Vehicles => Set<Vehicle>();
        public DbSet<Repair> Repairs => Set<Repair>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // INDISPENSABLE : configure les tables d'Identity (AspNetUsers, etc.).
            // L'oublier casse toute l'authentification.
            base.OnModelCreating(builder);

            // La configuration EF vit ici (couche Infrastructure) et non dans les
            // entités (couche Domain), qui restent de simples POCO sans dépendance à EF.

            builder.Entity<Brand>(entity =>
            {
                entity.HasIndex(m => m.Name).IsUnique();
            });

            builder.Entity<CarModel>(entity =>
            {
                // Un même nom de modèle peut exister chez deux marques, mais pas deux fois
                // chez la même.
                entity.HasIndex(m => new { m.BrandId, m.Name }).IsUnique();

                entity.HasOne(m => m.Brand)
                      .WithMany(ma => ma.CarModels)
                      .HasForeignKey(m => m.BrandId)
                      .OnDelete(DeleteBehavior.Restrict); // interdit de supprimer une marque utilisée
            });

            builder.Entity<Vehicle>(entity =>
            {
                entity.Property(v => v.PurchasePrice).HasPrecision(10, 2);

                // Unicité du VIN, mais seulement quand il est renseigné : sans ce filtre,
                // SQL Server refuserait deux véhicules sans VIN.
                entity.HasIndex(v => v.Vin)
                      .IsUnique()
                      .HasFilter("[Vin] IS NOT NULL");

                entity.HasOne(v => v.CarModel)
                      .WithMany(m => m.Vehicles)
                      .HasForeignKey(v => v.CarModelId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Repair>(entity =>
            {
                entity.Property(r => r.Cost).HasPrecision(10, 2);

                // Cascade : une réparation n'a aucun sens sans son véhicule.
                entity.HasOne(r => r.Vehicle)
                      .WithMany(v => v.Repairs)
                      .HasForeignKey(r => r.VehicleId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Données de départ issues de l'inventaire du client.
            builder.SeedInitialData();
        }
    }
}
