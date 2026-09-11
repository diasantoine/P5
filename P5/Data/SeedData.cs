using Microsoft.EntityFrameworkCore;
using P5.Models;

namespace P5.Data
{
    /// <summary>
    /// Données de départ reprises de l'inventaire transmis par le client
    /// (semaine du 7 au 13 avril 2022).
    ///
    /// Seed APPLICATIF : il s'exécute à chaque lancement (voir Program.cs), après
    /// l'application des migrations, et n'écrit que si la base est vide. Sur une base
    /// déjà remplie, il ne touche à rien : les données de Jacques ne sont jamais
    /// écrasées par les données d'exemple.
    ///
    /// Ce choix remplace l'ancien seed déclaratif (HasData dans OnModelCreating), qui
    /// figeait les données dans les migrations et se relisait mal. Les migrations ne
    /// décrivent plus que le schéma ; les données de départ vivent ici, en C# lisible.
    /// </summary>
    public static class SeedData
    {
        /// <summary>
        /// Insère l'inventaire de départ si, et seulement si, la base ne contient
        /// encore aucune marque. La marque est la racine du catalogue : une base qui
        /// en possède une a déjà été alimentée, par ce seed ou par l'utilisateur.
        /// </summary>
        public static async Task SeedAsync(ApplicationDbContext db)
        {
            // Protection : base non vide = on ne remplace rien.
            if (await db.Brands.AnyAsync())
            {
                return;
            }

            // Le graphe d'objets est construit de haut en bas (Marque > Modèle >
            // Finition > Véhicule > Réparation). EF Core insère le tout dans le bon
            // ordre et affecte lui-même les clés : aucun identifiant n'est codé en dur.
            var mazda      = new Brand { Name = "Mazda" };
            var jeep       = new Brand { Name = "Jeep" };
            var renault    = new Brand { Name = "Renault" };
            var ford       = new Brand { Name = "Ford" };
            var honda      = new Brand { Name = "Honda" };
            var volkswagen = new Brand { Name = "Volkswagen" };

            var miataLe       = Trim(mazda,      "Miata",    "LE");
            var libertySport  = Trim(jeep,       "Liberty",  "Sport");
            var scenicTce     = Trim(renault,    "Scénic",   "TCe");
            var explorerXlt   = Trim(ford,       "Explorer", "XLT");
            var civicLx       = Trim(honda,      "Civic",    "LX");
            var gtiS          = Trim(volkswagen, "GTI",      "S");
            var edgeSel       = Trim(ford,       "Edge",     "SEL"); // 2e Ford : la marque est partagée

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

        /// <summary>Crée un modèle chez une marque et l'une de ses finitions.</summary>
        private static Trim Trim(Brand brand, string carModelName, string trimName)
            => new() { Name = trimName, CarModel = new CarModel { Name = carModelName, Brand = brand } };

        /// <summary>Un véhicule de l'inventaire, avec sa réparation unique telle que la feuille la libelle.</summary>
        private static Vehicle Vehicle(
            Trim trim, int year, DateOnly purchaseDate, decimal purchasePrice,
            DateOnly availabilityDate, DateOnly? saleDate, string repairDescription, decimal repairCost)
            => new()
            {
                Trim = trim,
                Year = year,
                PurchaseDate = purchaseDate,
                PurchasePrice = purchasePrice,
                AvailabilityDate = availabilityDate,
                SaleDate = saleDate, // null = toujours disponible
                Repairs = [new Repair { Description = repairDescription, Cost = repairCost }]
            };
    }
}
