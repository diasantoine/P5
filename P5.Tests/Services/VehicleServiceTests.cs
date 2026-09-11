using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using P5.Configuration;
using P5.Data;
using P5.Models;
using P5.Services;

namespace P5.Tests.Services;

public class VehicleServiceTests
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
        context.Repairs.Add(new Repair { Id = 1, Description = "Carrosserie", Cost = 1100m, VehicleId = 1 });
        context.SaveChanges();

        return context;
    }

    [Fact]
    public async Task GetInventoryAsync_LoadsBrandAndRepairs()
    {
        using var context = CreateContext();
        var service = new VehicleService(context, Options.Create(new PricingOptions()));

        var inventory = await service.GetInventoryAsync();

        var vehicle = Assert.Single(inventory);
        Assert.Equal("Ford", vehicle.Specification?.Brand?.Name);
        Assert.Equal(1100m, vehicle.RepairsCost);
        Assert.Equal(25950m, vehicle.SalePrice);
    }

    [Fact]
    public async Task GetDetailAsync_ReturnsNull_WhenIdIsUnknown()
    {
        using var context = CreateContext();
        var service = new VehicleService(context, Options.Create(new PricingOptions()));

        Assert.Null(await service.GetDetailAsync(999));
    }

    [Fact]
    public async Task MarkAsSoldAsync_SetsSaleDate()
    {
        using var context = CreateContext();
        var service = new VehicleService(context, Options.Create(new PricingOptions()));

        var result = await service.MarkAsSoldAsync(1, new DateOnly(2026, 9, 4));

        Assert.True(result);
        Assert.Equal(new DateOnly(2026, 9, 4), context.Vehicles.Single().SaleDate);
    }

    [Fact]
    public async Task MarkAsSoldAsync_ReturnsFalse_WhenVehicleDoesNotExist()
    {
        using var context = CreateContext();
        var service = new VehicleService(context, Options.Create(new PricingOptions()));

        Assert.False(await service.MarkAsSoldAsync(999, new DateOnly(2026, 9, 4)));
    }

    [Fact]
    public async Task MarkAsSoldAsync_RefusesADateBeforePurchase()
    {
        using var context = CreateContext();
        var service = new VehicleService(context, Options.Create(new PricingOptions()));

        var result = await service.MarkAsSoldAsync(1, new DateOnly(2022, 4, 3));

        Assert.False(result);
        Assert.Null(context.Vehicles.Single().SaleDate);
    }

    [Fact]
    public async Task GetSpecificationOptionsAsync_LabelsBrandModelAndTrim()
    {
        using var context = CreateContext();
        var service = new VehicleService(context, Options.Create(new PricingOptions()));

        var options = await service.GetSpecificationOptionsAsync();

        Assert.Equal("Ford Explorer XLT", Assert.Single(options).Label);
    }

    [Fact]
    public async Task GetDetailAsync_AppliesTheConfiguredMargin()
    {
        using var context = CreateContext();
        var service = new VehicleService(context, Options.Create(new PricingOptions { FixedMargin = 700m }));

        var vehicle = await service.GetDetailAsync(1);

        Assert.Equal(700m, vehicle!.Margin);
        Assert.Equal(24350m + 1100m + 700m, vehicle.SalePrice);
    }
}
