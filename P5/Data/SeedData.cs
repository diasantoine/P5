using Microsoft.EntityFrameworkCore;
using P5.Models;

namespace P5.Data
{
    /// <summary>
    /// Données de départ reprises de l'inventaire transmis par le client (semaine du 7 au 13 avril 2022).
    /// Exécuté au lancement (voir Program.cs) ; ne fait rien si la base contient déjà une marque.
    /// </summary>
    public static class SeedData
    {
        /// <summary>La marque est la racine du catalogue : une base qui en possède une a déjà été alimentée, par ce seed ou par l'utilisateur.</summary>
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

            var miataLe       = Spec(mazda,      "Miata",    "LE");
            var libertySport  = Spec(jeep,       "Liberty",  "Sport");
            var scenicTce     = Spec(renault,    "Scénic",   "TCe");
            var explorerXlt   = Spec(ford,       "Explorer", "XLT");
            var civicLx       = Spec(honda,      "Civic",    "LX");
            var gtiS          = Spec(volkswagen, "GTI",      "S");
            var edgeSel       = Spec(ford,       "Edge",     "SEL"); // 2e Ford : la marque est partagée

            // La feuille de calcul ne donne qu'un COÛT TOTAL par véhicule, jamais le
            // détail par intervention. Le libellé source est donc conservé tel quel
            // plutôt que d'inventer une répartition des montants.
            db.Vehicles.AddRange(
                Vehicle(miataLe,      2019, new(2022, 1, 7), 1800m,  new(2022, 4, 7),  new(2022, 4, 8),  "Restauration complète",        7600m),
                Vehicle(libertySport, 2007, new(2022, 4, 2), 4500m,  new(2022, 4, 7),  new(2022, 4, 9),  "Roulements des roues avant",   350m),
                Vehicle(scenicTce,    2007, new(2022, 4, 4), 1800m,  new(2022, 4, 8),  null,             "Radiateur, freins",            690m),
                Vehicle(explorerXlt,  2017, new(2022, 4, 5), 24350m, new(2022, 4, 9),  null,             "Pneus, freins",                1100m),
                Vehicle(civicLx,      2008, new(2022, 4, 6), 4000m,  new(2022, 4, 9),  new(2022, 4, 9),  "Climatisation, freins",        475m),
                Vehicle(gtiS,         2016, new(2022, 4, 6), 15250m, new(2022, 4, 10), new(2022, 4, 12), "Pneus",                        440m),
                Vehicle(edgeSel,      2013, new(2022, 4, 7), 10990m, new(2022, 4, 11), new(2022, 4, 12), "Pneus, freins, climatisation", 950m));

            await db.SaveChangesAsync();
        }

        /// <summary>Crée un modèle chez une marque et l'une de ses finitions, réunis dans une spécification.</summary>
        private static VehicleSpecification Spec(Brand brand, string carModelName, string trimName)
        {
            var carModel = new CarModel { Name = carModelName, Brand = brand };
            var trim = new Trim { Name = trimName, CarModel = carModel };
            return new VehicleSpecification { Brand = brand, CarModel = carModel, Trim = trim };
        }

        /// <summary>Un véhicule de l'inventaire, avec sa réparation unique telle que la feuille la libelle.</summary>
        private static Vehicle Vehicle(
            VehicleSpecification spec, int year, DateOnly purchaseDate, decimal purchasePrice,
            DateOnly availabilityDate, DateOnly? saleDate, string repairDescription, decimal repairCost)
            => new()
            {
                Specification = spec,
                Year = year,
                PurchaseDate = purchaseDate,
                PurchasePrice = purchasePrice,
                AvailabilityDate = availabilityDate,
                SaleDate = saleDate,
                Repairs = [new Repair { Description = repairDescription, Cost = repairCost }]
            };
    }
}
