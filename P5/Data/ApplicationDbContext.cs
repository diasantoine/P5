using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using P5.Models;

namespace P5.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        // Un DbSet = une table. Sans DbSet, pas de table générée.
        public DbSet<Marque> Marques => Set<Marque>();
        public DbSet<ModeleVoiture> ModelesVoiture => Set<ModeleVoiture>();
        public DbSet<Vehicule> Vehicules => Set<Vehicule>();
        public DbSet<Reparation> Reparations => Set<Reparation>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // INDISPENSABLE : configure les tables d'Identity (AspNetUsers, etc.).
            // L'oublier casse toute l'authentification.
            base.OnModelCreating(builder);

            // La configuration EF vit ici (couche Infrastructure) et non dans les
            // entités (couche Domain), qui restent de simples POCO sans dépendance à EF.

            builder.Entity<Marque>(entity =>
            {
                entity.HasIndex(m => m.Nom).IsUnique();
            });

            builder.Entity<ModeleVoiture>(entity =>
            {
                // Un même nom de modèle peut exister chez deux marques, mais pas deux fois
                // chez la même.
                entity.HasIndex(m => new { m.MarqueId, m.Nom }).IsUnique();

                entity.HasOne(m => m.Marque)
                      .WithMany(ma => ma.Modeles)
                      .HasForeignKey(m => m.MarqueId)
                      .OnDelete(DeleteBehavior.Restrict); // interdit de supprimer une marque utilisée
            });

            builder.Entity<Vehicule>(entity =>
            {
                entity.Property(v => v.PrixAchat).HasPrecision(10, 2);

                // Unicité du VIN, mais seulement quand il est renseigné : sans ce filtre,
                // SQL Server refuserait deux véhicules sans VIN.
                entity.HasIndex(v => v.CodeVin)
                      .IsUnique()
                      .HasFilter("[CodeVin] IS NOT NULL");

                entity.HasOne(v => v.ModeleVoiture)
                      .WithMany(m => m.Vehicules)
                      .HasForeignKey(v => v.ModeleVoitureId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Reparation>(entity =>
            {
                entity.Property(r => r.Cout).HasPrecision(10, 2);

                // Cascade : une réparation n'a aucun sens sans son véhicule.
                entity.HasOne(r => r.Vehicule)
                      .WithMany(v => v.Reparations)
                      .HasForeignKey(r => r.VehiculeId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
