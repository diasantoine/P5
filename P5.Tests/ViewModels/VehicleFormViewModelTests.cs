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
}
