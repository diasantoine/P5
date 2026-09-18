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
        public DbSet<VehicleSpecification> VehicleSpecifications => Set<VehicleSpecification>();
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

                // Clé alternative : VehicleSpecification pointe le couple (modèle, marque),
                // ce qui interdit en base un modèle qui n'appartient pas à la marque choisie.
                entity.HasAlternateKey(m => new { m.Id, m.BrandId });

                entity.HasOne(m => m.Brand)
                      .WithMany(ma => ma.CarModels)
                      .HasForeignKey(m => m.BrandId)
                      .OnDelete(DeleteBehavior.Restrict); // interdit de supprimer une marque utilisée
            });

            builder.Entity<Trim>(entity =>
            {
                // Même règle qu'entre marque et modèle : « LE » peut exister chez deux modèles différents, mais pas deux fois pour le même modèle.
                entity.HasIndex(t => new { t.CarModelId, t.Name }).IsUnique();
                entity.HasAlternateKey(t => new { t.Id, t.CarModelId });

                entity.HasOne(t => t.CarModel)
                      .WithMany(m => m.Trims)
                      .HasForeignKey(t => t.CarModelId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<VehicleSpecification>(entity =>
            {
                entity.HasIndex(s => new { s.BrandId, s.CarModelId, s.TrimId }).IsUnique();

                entity.HasOne(s => s.Brand)
                      .WithMany(b => b.Specifications)
                      .HasForeignKey(s => s.BrandId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Clés étrangères composites : la base refuse un modèle qui n'est pas
                // de cette marque, et une finition qui n'est pas de ce modèle.
                entity.HasOne(s => s.CarModel)
                      .WithMany(m => m.Specifications)
                      .HasForeignKey(s => new { s.CarModelId, s.BrandId })
                      .HasPrincipalKey(m => new { m.Id, m.BrandId })
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Trim)
                      .WithMany(t => t.Specifications)
                      .HasForeignKey(s => new { s.TrimId, s.CarModelId })
                      .HasPrincipalKey(t => new { t.Id, t.CarModelId })
                      .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Vehicle>(entity =>
            {
                entity.Property(v => v.PurchasePrice).HasPrecision(10, 2);

                // Unicité du VIN uniquement quand il est renseigné : plusieurs véhicules peuvent rester sans VIN.
                entity.HasIndex(v => v.Vin)
                      .IsUnique()
                      .HasFilter("[Vin] IS NOT NULL");

                entity.HasOne(v => v.Specification)
                      .WithMany(s => s.Vehicles)
                      .HasForeignKey(v => v.SpecificationId)
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
