using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using P5.Configuration;
using P5.Data;
using P5.Models;
using P5.Services;

namespace P5.Tests.Services;

public class RepairServiceTests
{
    private static ApplicationDbContext CreateContext()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection, contextOwnsConnection: true)
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();

        var brand = new Brand { Id = 1, Name = "Ford" };
        var carModel = new CarModel { Id = 1, Name = "Explorer", BrandId = 1, Brand = brand };
        var trim = new Trim { Id = 1, Name = "XLT", CarModelId = 1, CarModel = carModel };
        var spec = new VehicleSpecification { Id = 1, BrandId = 1, Brand = brand, CarModelId = 1, CarModel = carModel, TrimId = 1, Trim = trim };
        context.Brands.Add(brand);
        context.CarModels.Add(carModel);
        context.Trims.Add(trim);
        context.VehicleSpecifications.Add(spec);
        context.Vehicles.Add(new Vehicle
        {
            Id = 1,
            Year = 2017,
            SpecificationId = 1,
            PurchaseDate = new DateOnly(2022, 4, 4),
            PurchasePrice = 24350m
        });
        context.SaveChanges();
        return context;
    }

    [Fact]
    public async Task AddAsync_RaisesTheSalePrice()
    {
        using var context = CreateContext();
        var repairs = new RepairService(context);
        var vehicles = new VehicleService(context, Options.Create(new PricingOptions()));

        await repairs.AddAsync(new Repair { Description = "Carrosserie", Cost = 1100m, VehicleId = 1 });

        var vehicle = await vehicles.GetDetailAsync(1);
        Assert.Equal(25950m, vehicle!.SalePrice);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsTheVehicleId()
    {
        using var context = CreateContext();
        var service = new RepairService(context);
        var id = await service.AddAsync(new Repair { Description = "Pneus", Cost = 300m, VehicleId = 1 });

        Assert.Equal(1, await service.DeleteAsync(id));
        Assert.Empty(context.Repairs);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNull_WhenRepairDoesNotExist()
    {
        using var context = CreateContext();
        var service = new RepairService(context);

        Assert.Null(await service.DeleteAsync(999));
    }

    [Fact]
    public async Task VehicleExistsAsync_DistinguishesAnUnknownId()
    {
        using var context = CreateContext();
        var service = new RepairService(context);

        Assert.True(await service.VehicleExistsAsync(1));
        Assert.False(await service.VehicleExistsAsync(999));
    }
}
