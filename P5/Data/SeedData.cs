using Microsoft.EntityFrameworkCore;
using P5.Models;

namespace P5.Data
{
    /// <summary>
    /// Données de départ reprises de l'inventaire transmis par le client
    /// (semaine du 7 au 13 avril 2022).
    /// Insérées par la migration : un clone du dépôt suivi d'un "database update"
    /// reproduit exactement la même base.
    /// </summary>
    public static class SeedData
    {
        public static void AppliquerDonneesInitiales(this ModelBuilder builder)
        {
            // HasData impose des clés primaires explicites : EF doit pouvoir
            // comparer l'existant au souhaité pour générer les INSERT/UPDATE/DELETE.

            builder.Entity<Marque>().HasData(
                new Marque { Id = 1, Nom = "Mazda" },
                new Marque { Id = 2, Nom = "Jeep" },
                new Marque { Id = 3, Nom = "Renault" },
                new Marque { Id = 4, Nom = "Ford" },
                new Marque { Id = 5, Nom = "Honda" },
                new Marque { Id = 6, Nom = "Volkswagen" });

            builder.Entity<ModeleVoiture>().HasData(
                new ModeleVoiture { Id = 1, Nom = "Miata",    MarqueId = 1 },
                new ModeleVoiture { Id = 2, Nom = "Liberty",  MarqueId = 2 },
                new ModeleVoiture { Id = 3, Nom = "Scénic",   MarqueId = 3 },
                new ModeleVoiture { Id = 4, Nom = "Explorer", MarqueId = 4 },
                new ModeleVoiture { Id = 5, Nom = "Civic",    MarqueId = 5 },
                new ModeleVoiture { Id = 6, Nom = "GTI",      MarqueId = 6 },
                new ModeleVoiture { Id = 7, Nom = "Edge",     MarqueId = 4 }); // 2e Ford

            builder.Entity<Vehicule>().HasData(
                new Vehicule
                {
                    Id = 1, Annee = 2019, ModeleVoitureId = 1, Finition = "LE",
                    DateAchat = new DateOnly(2022, 1, 7), PrixAchat = 1800m,
                    DateDisponibilite = new DateOnly(2022, 4, 7),
                    DateVente = new DateOnly(2022, 4, 8)
                },
                new Vehicule
                {
                    Id = 2, Annee = 2007, ModeleVoitureId = 2, Finition = "Sport",
                    DateAchat = new DateOnly(2022, 4, 2), PrixAchat = 4500m,
                    DateDisponibilite = new DateOnly(2022, 4, 7),
                    DateVente = new DateOnly(2022, 4, 9)
                },
                new Vehicule
                {
                    Id = 3, Annee = 2007, ModeleVoitureId = 3, Finition = "TCe",
                    DateAchat = new DateOnly(2022, 4, 4), PrixAchat = 1800m,
                    DateDisponibilite = new DateOnly(2022, 4, 8),
                    DateVente = null // toujours disponible
                },
                new Vehicule
                {
                    Id = 4, Annee = 2017, ModeleVoitureId = 4, Finition = "XLT",
                    DateAchat = new DateOnly(2022, 4, 5), PrixAchat = 24350m,
                    DateDisponibilite = new DateOnly(2022, 4, 9),
                    DateVente = null // toujours disponible
                },
                new Vehicule
                {
                    Id = 5, Annee = 2008, ModeleVoitureId = 5, Finition = "LX",
                    DateAchat = new DateOnly(2022, 4, 6), PrixAchat = 4000m,
                    DateDisponibilite = new DateOnly(2022, 4, 9),
                    DateVente = new DateOnly(2022, 4, 9)
                },
                new Vehicule
                {
                    Id = 6, Annee = 2016, ModeleVoitureId = 6, Finition = "S",
                    DateAchat = new DateOnly(2022, 4, 6), PrixAchat = 15250m,
                    DateDisponibilite = new DateOnly(2022, 4, 10),
                    DateVente = new DateOnly(2022, 4, 12)
                },
                new Vehicule
                {
                    Id = 7, Annee = 2013, ModeleVoitureId = 7, Finition = "SEL",
                    DateAchat = new DateOnly(2022, 4, 7), PrixAchat = 10990m,
                    DateDisponibilite = new DateOnly(2022, 4, 11),
                    DateVente = new DateOnly(2022, 4, 12)
                });

            // La feuille de calcul ne donne qu'un COÛT TOTAL par véhicule, jamais le
            // détail par intervention. Le libellé source est donc conservé tel quel
            // plutôt que d'inventer une répartition des montants.
            builder.Entity<Reparation>().HasData(
                new Reparation { Id = 1, VehiculeId = 1, Libelle = "Restauration complète",         Cout = 7600m },
                new Reparation { Id = 2, VehiculeId = 2, Libelle = "Roulements des roues avant",    Cout = 350m },
                new Reparation { Id = 3, VehiculeId = 3, Libelle = "Radiateur, freins",             Cout = 690m },
                new Reparation { Id = 4, VehiculeId = 4, Libelle = "Pneus, freins",                 Cout = 1100m },
                new Reparation { Id = 5, VehiculeId = 5, Libelle = "Climatisation, freins",         Cout = 475m },
                new Reparation { Id = 6, VehiculeId = 6, Libelle = "Pneus",                         Cout = 440m },
                new Reparation { Id = 7, VehiculeId = 7, Libelle = "Pneus, freins, climatisation",  Cout = 950m });
        }
    }
}
