using P5.Models;
using P5.ViewModels;

namespace P5.Tests.ViewModels;

public class VehicleFormViewModelTests
{
    [Fact]
    public void ApplyTo_NeverTouchesSaleDate()
    {
        var form = new VehicleFormViewModel();
        var v = new Vehicle { SaleDate = new DateOnly(2022, 4, 8) };

        form.ApplyTo(v);

        Assert.Equal(new DateOnly(2022, 4, 8), v.SaleDate);
    }

    [Fact]
    public void ApplyTo_NeverTouchesRepairs()
    {
        var form = new VehicleFormViewModel();
        var repair = new Repair { Description = "Carrosserie", Cost = 1100m };
        var v = new Vehicle { Repairs = [repair] };

        form.ApplyTo(v);

        Assert.Same(repair, Assert.Single(v.Repairs));
    }

    // Margin et RepairsCost ne servent qu'a l'apercu du prix de vente : un formulaire forge ne doit pas pouvoir les poster.
    [Theory]
    [InlineData(nameof(VehicleFormViewModel.Margin))]
    [InlineData(nameof(VehicleFormViewModel.RepairsCost))]
    [InlineData(nameof(VehicleFormViewModel.Specifications))]
    public void DisplayOnlyProperties_AreNeverBound(string propertyName)
    {
        var property = typeof(VehicleFormViewModel).GetProperty(propertyName)!;

        Assert.NotEmpty(property.GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.BindNeverAttribute), inherit: true));
    }

    [Fact]
    public void ApplyTo_IgnoresThePreviewMargin()
    {
        var form = new VehicleFormViewModel { Margin = 9999m };
        var v = new Vehicle { Margin = 500m };

        form.ApplyTo(v);

        Assert.Equal(500m, v.Margin);
    }
}
