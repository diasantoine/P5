using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using P5.Data;
using P5.Models;

namespace P5.Tests.Data;

/// <summary>Le seed applicatif s'exécute au lancement de l'application.</summary>
public sealed class SeedDataTests : IDisposable
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    public SeedDataTests()
    {
        _connection.Open();
        using var db = NewContext();
        db.Database.EnsureCreated();
    }

    public void Dispose() => _connection.Dispose();

    private ApplicationDbContext NewContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;
        return new ApplicationDbContext(options);
    }

    /// <summary>Les 7 lignes de l'inventaire du client, telles que le site doit les afficher.</summary>
    [Theory]
    [InlineData("Mazda", "Miata", "LE", 2019, 9900)]
    [InlineData("Jeep", "Liberty", "Sport", 2007, 5350)]
    [InlineData("Renault", "Scénic", "TCe", 2007, 2990)]
    [InlineData("Ford", "Explorer", "XLT", 2017, 25950)]
    [InlineData("Honda", "Civic", "LX", 2008, 4975)]
    [InlineData("Volkswagen", "GTI", "S", 2016, 16190)]
    [InlineData("Ford", "Edge", "SEL", 2013, 12440)]
    public async Task SeedAsync_OnEmptyDatabase_ReproducesClientInventory(
        string brand, string carModel, string trim, int year, decimal salePrice)
    {
        using (var db = NewContext())
        {
            await SeedData.SeedAsync(db);
        }

        using var check = NewContext();
        var vehicle = await check.Vehicles
            .Include(v => v.Specification!).ThenInclude(s => s.Brand)
            .Include(v => v.Specification!).ThenInclude(s => s.CarModel)
            .Include(v => v.Specification!).ThenInclude(s => s.Trim)
            .Include(v => v.Repairs)
            .SingleAsync(v => v.Specification!.Trim!.Name == trim && v.Specification.CarModel!.Name == carModel);

        Assert.Equal(brand, vehicle.Specification!.Brand!.Name);
        Assert.Equal(year, vehicle.Year);
        Assert.Equal(salePrice, vehicle.SalePrice);
    }

    [Fact]
    public async Task SeedAsync_OnEmptyDatabase_InsertsExactlyTheInventory()
    {
        using (var db = NewContext())
        {
            await SeedData.SeedAsync(db);
        }

        using var check = NewContext();
        Assert.Equal(6, await check.Brands.CountAsync());   // Ford est partagé par deux véhicules
        Assert.Equal(7, await check.CarModels.CountAsync());
        Assert.Equal(7, await check.Trims.CountAsync());
        Assert.Equal(7, await check.VehicleSpecifications.CountAsync());
        Assert.Equal(7, await check.Vehicles.CountAsync());
        Assert.Equal(7, await check.Repairs.CountAsync());
    }

    [Fact]
    public async Task SeedAsync_OnDatabaseThatAlreadyHasData_ChangesNothing()
    {
        using (var db = NewContext())
        {
            db.Brands.Add(new Brand { Name = "Peugeot" });
            await db.SaveChangesAsync();
        }

        using (var db = NewContext())
        {
            await SeedData.SeedAsync(db);
        }

        using var check = NewContext();
        Assert.Equal("Peugeot", (await check.Brands.SingleAsync()).Name);
        Assert.Equal(0, await check.Vehicles.CountAsync());
    }

    [Fact]
    public async Task SeedAsync_CalledTwice_DoesNotDuplicateTheInventory()
    {
        using (var db = NewContext())
        {
            await SeedData.SeedAsync(db);
        }

        using (var db = NewContext())
        {
            await SeedData.SeedAsync(db);
        }

        using var check = NewContext();
        Assert.Equal(7, await check.Vehicles.CountAsync());
        Assert.Equal(6, await check.Brands.CountAsync());
    }
}
