using P5.Models;

namespace P5.Tests.Models;

public class VehicleTests
{
    /// <summary>
    /// Les 7 lignes de l'inventaire transmis par le client, avec leur prix de vente
    /// attendu. Si un jour la marge change, c'est ici que le projet doit crier.
    /// </summary>
    public static TheoryData<decimal, decimal[], decimal> Inventory => new()
    {
        { 1800m,  [7600m], 9900m },
        { 4500m,  [350m],  5350m },
        { 1800m,  [690m],  2990m },
        { 24350m, [1100m], 25950m },
        { 4000m,  [475m],  4975m },
        { 15250m, [440m],  16190m },
        { 10990m, [950m],  12440m },
    };

    [Theory]
    [MemberData(nameof(Inventory))]
    public void SalePrice_ReproducesClientInventory(decimal purchasePrice, decimal[] costs, decimal expected)
    {
        var vehicle = new Vehicle
        {
            PurchasePrice = purchasePrice,
            Repairs = [.. costs.Select(c => new Repair { Description = "Reparation", Cost = c })]
        };

        Assert.Equal(expected, vehicle.SalePrice);
    }

    [Fact]
    public void SalePrice_WithoutRepairs_IsPurchasePricePlusMargin()
    {
        var vehicle = new Vehicle { PurchasePrice = 1000m };

        Assert.Equal(0m, vehicle.RepairsCost);
        Assert.Equal(1500m, vehicle.SalePrice);
    }

    [Fact]
    public void SalePrice_SumsAllRepairs()
    {
        var vehicle = new Vehicle
        {
            PurchasePrice = 1000m,
            Repairs =
            [
                new Repair { Description = "Pneus", Cost = 300m },
                new Repair { Description = "Freins", Cost = 200m }
            ]
        };

        Assert.Equal(500m, vehicle.RepairsCost);
        Assert.Equal(2000m, vehicle.SalePrice);
    }

    [Fact]
    public void IsAvailable_IsTrue_WhileSaleDateIsNull()
    {
        var vehicle = new Vehicle { SaleDate = null };
        Assert.True(vehicle.IsAvailable);
    }

    [Fact]
    public void IsAvailable_IsFalse_AsSoonAsSaleDateIsSet()
    {
        var vehicle = new Vehicle { SaleDate = new DateOnly(2022, 4, 8) };
        Assert.False(vehicle.IsAvailable);
    }

    [Fact]
    public void SalePrice_UsesTheDefaultMargin_WhenNoneIsApplied()
    {
        Assert.Equal(500m, new Vehicle().Margin);
    }
}
