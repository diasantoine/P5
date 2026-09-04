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
        public static void SeedInitialData(this ModelBuilder builder)
        {
            // HasData impose des clés primaires explicites : EF doit pouvoir
            // comparer l'existant au souhaité pour générer les INSERT/UPDATE/DELETE.

            builder.Entity<Brand>().HasData(
                new Brand { Id = 1, Name = "Mazda" },
                new Brand { Id = 2, Name = "Jeep" },
                new Brand { Id = 3, Name = "Renault" },
                new Brand { Id = 4, Name = "Ford" },
                new Brand { Id = 5, Name = "Honda" },
                new Brand { Id = 6, Name = "Volkswagen" });

            builder.Entity<CarModel>().HasData(
                new CarModel { Id = 1, Name = "Miata",    BrandId = 1 },
                new CarModel { Id = 2, Name = "Liberty",  BrandId = 2 },
                new CarModel { Id = 3, Name = "Scénic",   BrandId = 3 },
                new CarModel { Id = 4, Name = "Explorer", BrandId = 4 },
                new CarModel { Id = 5, Name = "Civic",    BrandId = 5 },
                new CarModel { Id = 6, Name = "GTI",      BrandId = 6 },
                new CarModel { Id = 7, Name = "Edge",     BrandId = 4 }); // 2e Ford

            builder.Entity<Vehicle>().HasData(
                new Vehicle
                {
                    Id = 1, Year = 2019, CarModelId = 1, Trim = "LE",
                    PurchaseDate = new DateOnly(2022, 1, 7), PurchasePrice = 1800m,
                    AvailabilityDate = new DateOnly(2022, 4, 7),
                    SaleDate = new DateOnly(2022, 4, 8)
                },
                new Vehicle
                {
                    Id = 2, Year = 2007, CarModelId = 2, Trim = "Sport",
                    PurchaseDate = new DateOnly(2022, 4, 2), PurchasePrice = 4500m,
                    AvailabilityDate = new DateOnly(2022, 4, 7),
                    SaleDate = new DateOnly(2022, 4, 9)
                },
                new Vehicle
                {
                    Id = 3, Year = 2007, CarModelId = 3, Trim = "TCe",
                    PurchaseDate = new DateOnly(2022, 4, 4), PurchasePrice = 1800m,
                    AvailabilityDate = new DateOnly(2022, 4, 8),
                    SaleDate = null // toujours disponible
                },
                new Vehicle
                {
                    Id = 4, Year = 2017, CarModelId = 4, Trim = "XLT",
                    PurchaseDate = new DateOnly(2022, 4, 5), PurchasePrice = 24350m,
                    AvailabilityDate = new DateOnly(2022, 4, 9),
                    SaleDate = null // toujours disponible
                },
                new Vehicle
                {
                    Id = 5, Year = 2008, CarModelId = 5, Trim = "LX",
                    PurchaseDate = new DateOnly(2022, 4, 6), PurchasePrice = 4000m,
                    AvailabilityDate = new DateOnly(2022, 4, 9),
                    SaleDate = new DateOnly(2022, 4, 9)
                },
                new Vehicle
                {
                    Id = 6, Year = 2016, CarModelId = 6, Trim = "S",
                    PurchaseDate = new DateOnly(2022, 4, 6), PurchasePrice = 15250m,
                    AvailabilityDate = new DateOnly(2022, 4, 10),
                    SaleDate = new DateOnly(2022, 4, 12)
                },
                new Vehicle
                {
                    Id = 7, Year = 2013, CarModelId = 7, Trim = "SEL",
                    PurchaseDate = new DateOnly(2022, 4, 7), PurchasePrice = 10990m,
                    AvailabilityDate = new DateOnly(2022, 4, 11),
                    SaleDate = new DateOnly(2022, 4, 12)
                });

            // La feuille de calcul ne donne qu'un COÛT TOTAL par véhicule, jamais le
            // détail par intervention. Le libellé source est donc conservé tel quel
            // plutôt que d'inventer une répartition des montants.
            builder.Entity<Repair>().HasData(
                new Repair { Id = 1, VehicleId = 1, Description = "Restauration complète",         Cost = 7600m },
                new Repair { Id = 2, VehicleId = 2, Description = "Roulements des roues avant",    Cost = 350m },
                new Repair { Id = 3, VehicleId = 3, Description = "Radiateur, freins",             Cost = 690m },
                new Repair { Id = 4, VehicleId = 4, Description = "Pneus, freins",                 Cost = 1100m },
                new Repair { Id = 5, VehicleId = 5, Description = "Climatisation, freins",         Cost = 475m },
                new Repair { Id = 6, VehicleId = 6, Description = "Pneus",                         Cost = 440m },
                new Repair { Id = 7, VehicleId = 7, Description = "Pneus, freins, climatisation",  Cost = 950m });
        }
    }
}
