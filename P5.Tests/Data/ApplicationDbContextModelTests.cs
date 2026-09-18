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
    public void Vehicle_HasASingleForeignKey_TowardItsSpecification()
    {
        var vehicle = BuildModel().FindEntityType(typeof(Vehicle))!;

        var toSpecification = Assert.Single(vehicle.GetForeignKeys());

        Assert.Equal(typeof(VehicleSpecification), toSpecification.PrincipalEntityType.ClrType);
        Assert.True(toSpecification.IsRequired);
        Assert.Equal(DeleteBehavior.Restrict, toSpecification.DeleteBehavior);
    }

    [Fact]
    public void Specification_CannotPairAModelWithAnotherBrand()
    {
        var specification = BuildModel().FindEntityType(typeof(VehicleSpecification))!;

        var toCarModel = Assert.Single(specification.GetForeignKeys(), f => f.PrincipalEntityType.ClrType == typeof(CarModel));

        Assert.Equal(["CarModelId", "BrandId"], toCarModel.Properties.Select(p => p.Name));
        Assert.Equal(["Id", "BrandId"], toCarModel.PrincipalKey.Properties.Select(p => p.Name));
    }

    [Fact]
    public void Specification_CannotPairATrimWithAnotherModel()
    {
        var specification = BuildModel().FindEntityType(typeof(VehicleSpecification))!;

        var toTrim = Assert.Single(specification.GetForeignKeys(), f => f.PrincipalEntityType.ClrType == typeof(Trim));

        Assert.Equal(["TrimId", "CarModelId"], toTrim.Properties.Select(p => p.Name));
        Assert.Equal(["Id", "CarModelId"], toTrim.PrincipalKey.Properties.Select(p => p.Name));
    }

    [Fact]
    public void Specification_TripletIsUnique()
    {
        var specification = BuildModel().FindEntityType(typeof(VehicleSpecification))!;

        var uniqueIndex = Assert.Single(specification.GetIndexes(), i => i.IsUnique);

        Assert.Equal(["BrandId", "CarModelId", "TrimId"], uniqueIndex.Properties.Select(p => p.Name));
    }

    /// <summary>
    /// Les clés étrangères composites empêchent d'enregistrer une spécification
    /// dont le modèle n'appartient pas à la marque déclarée.
    /// </summary>
    [Fact]
    public async Task Specification_CannotPersistAModelThatBelongsToAnotherBrand()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        using (var context = new ApplicationDbContext(options))
        {
            context.Database.EnsureCreated();

            var ford = new Brand { Id = 1, Name = "Ford" };
            var renault = new Brand { Id = 2, Name = "Renault" };
            var explorer = new CarModel { Id = 1, Name = "Explorer", BrandId = 1 };
            var xlt = new Trim { Id = 1, Name = "XLT", CarModelId = 1 };
            context.Brands.AddRange(ford, renault);
            context.CarModels.Add(explorer);
            context.Trims.Add(xlt);
            await context.SaveChangesAsync();

            // Explorer (id 1) appartient à Ford (id 1) : le rattacher à Renault (id 2)
            // viole la clé étrangère composite (CarModelId, BrandId) -> (Id, BrandId).
            context.VehicleSpecifications.Add(new VehicleSpecification { BrandId = 2, CarModelId = 1, TrimId = 1 });

            await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        }
    }
}
