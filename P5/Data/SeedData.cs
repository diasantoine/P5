using Microsoft.EntityFrameworkCore;
using P5.Models;

namespace P5.Data
{
    /// <summary>
    /// Données de départ de l'inventaire (semaine du 7 au 13 avril 2022).
    /// Exécuté au lancement (voir Program.cs) ; ne fait rien si la base contient déjà une marque.
    /// </summary>
    public static class SeedData
    {
        /// <summary>Une base qui possède déjà une marque a déjà été alimentée.</summary>
        public static async Task SeedAsync(ApplicationDbContext db)
        {
            if (await db.Brands.AnyAsync())
            {
                return;
            }

            // Le graphe d'objets est construit de haut en bas (Marque > Modèle > Finition > Véhicule > Réparation).
            var mazda      = new Brand { Name = "Mazda" };
            var jeep       = new Brand { Name = "Jeep" };
            var renault    = new Brand { Name = "Renault" };
            var ford       = new Brand { Name = "Ford" };
            var honda      = new Brand { Name = "Honda" };
            var volkswagen = new Brand { Name = "Volkswagen" };

            var miataLe       = NewTrim(mazda,      "Miata",    "LE");
            var libertySport  = NewTrim(jeep,       "Liberty",  "Sport");
            var scenicTce     = NewTrim(renault,    "Scénic",   "TCe");
            var explorerXlt   = NewTrim(ford,       "Explorer", "XLT");
            var civicLx       = NewTrim(honda,      "Civic",    "LX");
            var gtiS          = NewTrim(volkswagen, "GTI",      "S");
            // Deuxième Ford : la marque est partagée.
            var edgeSel       = NewTrim(ford,       "Edge",     "SEL");

            // Chaque véhicule n'a qu'une réparation, avec un libellé et un coût global, et une photo servie depuis wwwroot/images/vehicles.
            db.Vehicles.AddRange(
                Vehicle(miataLe,      2019, new(2022, 1, 7), 1800m,  new(2022, 4, 7),  new(2022, 4, 8),  "Restauration complète",        7600m, "miata"),
                Vehicle(libertySport, 2007, new(2022, 4, 2), 4500m,  new(2022, 4, 7),  new(2022, 4, 9),  "Roulements des roues avant",   350m,  "liberty"),
                Vehicle(scenicTce,    2007, new(2022, 4, 4), 1800m,  new(2022, 4, 8),  null,             "Radiateur, freins",            690m,  "scenic"),
                Vehicle(explorerXlt,  2017, new(2022, 4, 5), 24350m, new(2022, 4, 9),  null,             "Pneus, freins",                1100m, "explorer"),
                Vehicle(civicLx,      2008, new(2022, 4, 6), 4000m,  new(2022, 4, 9),  new(2022, 4, 9),  "Climatisation, freins",        475m,  "civic"),
                Vehicle(gtiS,         2016, new(2022, 4, 6), 15250m, new(2022, 4, 10), new(2022, 4, 12), "Pneus",                        440m,  "gti"),
                Vehicle(edgeSel,      2013, new(2022, 4, 7), 10990m, new(2022, 4, 11), new(2022, 4, 12), "Pneus, freins, climatisation", 950m,  "edge"));

            await db.SaveChangesAsync();
        }

        /// <summary>Crée un modèle chez une marque et l'une de ses finitions.</summary>
        private static Trim NewTrim(Brand brand, string carModelName, string trimName)
        {
            var carModel = new CarModel { Name = carModelName, Brand = brand };
            return new Trim { Name = trimName, CarModel = carModel };
        }

        /// <summary>Un véhicule de l'inventaire, avec sa réparation unique et sa photo.</summary>
        private static Vehicle Vehicle(
            Trim trim, int year, DateOnly purchaseDate, decimal purchasePrice,
            DateOnly availabilityDate, DateOnly? saleDate, string repairDescription, decimal repairCost,
            string photoName)
            => new()
            {
                Trim = trim,
                Year = year,
                PurchaseDate = purchaseDate,
                PurchasePrice = purchasePrice,
                AvailabilityDate = availabilityDate,
                SaleDate = saleDate,
                PhotoUrl = $"/images/vehicles/{photoName}.jpg",
                Repairs = [new Repair { Description = repairDescription, Cost = repairCost }]
            };
    }
}
