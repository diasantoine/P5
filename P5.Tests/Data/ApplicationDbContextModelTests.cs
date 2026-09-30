using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using P5.Data;
using P5.Models;

namespace P5.Tests.Data;

public class ApplicationDbContextModelTests
{
    private static IModel BuildModel()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=P5_ModelOnly")
            .Options;

        using var context = new ApplicationDbContext(options);
        return context.Model;
    }

    [Fact]
    public void Trim_NameIsUniquePerCarModel()
    {
        var trim = BuildModel().FindEntityType(typeof(Trim))!;

        var uniqueIndex = Assert.Single(trim.GetIndexes(), i => i.IsUnique);

        Assert.Equal(["CarModelId", "Name"], uniqueIndex.Properties.Select(p => p.Name));
    }

    [Fact]
    public void CarModel_NameIsUniquePerBrand()
    {
        var carModel = BuildModel().FindEntityType(typeof(CarModel))!;

        var uniqueIndex = Assert.Single(carModel.GetIndexes(), i => i.IsUnique);

        Assert.Equal(["BrandId", "Name"], uniqueIndex.Properties.Select(p => p.Name));
    }

    [Fact]
    public void Vehicle_HasASingleForeignKey_TowardItsTrim()
    {
        var vehicle = BuildModel().FindEntityType(typeof(Vehicle))!;

        var toTrim = Assert.Single(vehicle.GetForeignKeys());

        Assert.Equal(typeof(Trim), toTrim.PrincipalEntityType.ClrType);
        Assert.Equal(["TrimId"], toTrim.Properties.Select(p => p.Name));
        Assert.True(toTrim.IsRequired);
        Assert.Equal(DeleteBehavior.Restrict, toTrim.DeleteBehavior);
    }

    [Theory]
    [InlineData(typeof(CarModel), typeof(Brand), "BrandId")]
    [InlineData(typeof(Trim), typeof(CarModel), "CarModelId")]
    public void Catalogue_EachLevelPointsOnlyToItsParent(Type child, Type parent, string foreignKey)
    {
        var entity = BuildModel().FindEntityType(child)!;

        var toParent = Assert.Single(entity.GetForeignKeys());

        Assert.Equal(parent, toParent.PrincipalEntityType.ClrType);
        Assert.Equal([foreignKey], toParent.Properties.Select(p => p.Name));
        Assert.Equal(DeleteBehavior.Restrict, toParent.DeleteBehavior);
    }

    [Fact]
    public void Vehicle_StoresNeitherBrandNorModel()
    {
        var model = BuildModel();
        var vehicle = model.FindEntityType(typeof(Vehicle))!;

        Assert.Null(vehicle.FindProperty("BrandId"));
        Assert.Null(vehicle.FindProperty("CarModelId"));
        Assert.DoesNotContain(model.GetEntityTypes(), e => e.ClrType.Name == "VehicleSpecification");
    }

    [Fact]
    public async Task Trim_UsedByAVehicle_CannotBeDeleted()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        using (var context = new ApplicationDbContext(options))
        {
            context.Database.EnsureCreated();

            var ford = new Brand { Name = "Ford" };
            var explorer = new CarModel { Name = "Explorer", Brand = ford };
            var xlt = new Trim { Name = "XLT", CarModel = explorer };
            context.Vehicles.Add(new Vehicle { Trim = xlt, Year = 2017, PurchaseDate = new(2022, 4, 5), PurchasePrice = 24350m });
            await context.SaveChangesAsync();
        }

        // Un second contexte ne suit que la finition : c'est la base qui refuse la suppression.
        using var check = new ApplicationDbContext(options);
        check.Trims.Remove(check.Trims.Single());

        await Assert.ThrowsAsync<DbUpdateException>(() => check.SaveChangesAsync());
    }
}
