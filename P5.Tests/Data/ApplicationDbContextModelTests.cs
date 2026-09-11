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
    public void Vehicle_RequiresATrim_ThatCannotBeDeletedWhileInUse()
    {
        var vehicle = BuildModel().FindEntityType(typeof(Vehicle))!;

        var toTrim = Assert.Single(vehicle.GetForeignKeys(), f => f.PrincipalEntityType.ClrType == typeof(Trim));

        Assert.True(toTrim.IsRequired);
        Assert.Equal(DeleteBehavior.Restrict, toTrim.DeleteBehavior);
    }

    [Fact]
    public void Vehicle_ReachesItsCarModelOnlyThroughItsTrim()
    {
        var vehicle = BuildModel().FindEntityType(typeof(Vehicle))!;

        Assert.DoesNotContain(vehicle.GetForeignKeys(), f => f.PrincipalEntityType.ClrType == typeof(CarModel));
    }
}
