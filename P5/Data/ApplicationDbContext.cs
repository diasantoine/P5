using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using P5.Models;

namespace P5.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<Brand> Brands => Set<Brand>();
        public DbSet<CarModel> CarModels => Set<CarModel>();
        public DbSet<Trim> Trims => Set<Trim>();
        public DbSet<Vehicle> Vehicles => Set<Vehicle>();
        public DbSet<Repair> Repairs => Set<Repair>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // base.OnModelCreating configure les tables d'Identity (AspNetUsers, etc.).
            base.OnModelCreating(builder);

            builder.Entity<Brand>(entity =>
            {
                entity.HasIndex(m => m.Name).IsUnique();
            });

            builder.Entity<CarModel>(entity =>
            {
                // Un même nom de modèle peut exister chez deux marques, mais pas deux fois chez la même.
                entity.HasIndex(m => new { m.BrandId, m.Name }).IsUnique();

                // Restrict interdit de supprimer une marque utilisée.
                entity.HasOne(m => m.Brand)
                      .WithMany(ma => ma.CarModels)
                      .HasForeignKey(m => m.BrandId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Trim>(entity =>
            {
                // Même règle qu'entre marque et modèle : « LE » peut exister chez deux modèles différents, mais pas deux fois pour le même modèle.
                entity.HasIndex(t => new { t.CarModelId, t.Name }).IsUnique();

                entity.HasOne(t => t.CarModel)
                      .WithMany(m => m.Trims)
                      .HasForeignKey(t => t.CarModelId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Vehicle>(entity =>
            {
                entity.Property(v => v.PurchasePrice).HasPrecision(10, 2);

                // Unicité du VIN uniquement quand il est renseigné : plusieurs véhicules peuvent rester sans VIN.
                entity.HasIndex(v => v.Vin)
                      .IsUnique()
                      .HasFilter("[Vin] IS NOT NULL");

                // Restrict interdit de supprimer une finition utilisée.
                entity.HasOne(v => v.Trim)
                      .WithMany(t => t.Vehicles)
                      .HasForeignKey(v => v.TrimId)
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

            // Les données de départ sont insérées au lancement par SeedData.SeedAsync (voir Program.cs).
        }
    }
}
