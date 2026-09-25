using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using P5.Configuration;
using P5.Controllers;
using P5.Models;
using P5.Services;

namespace P5.Tests.Controllers;

// Vérifie que « Marquer comme vendu » exige une date de vente saisie, et la transmet au service.
public class VehiclesControllerTests
{
    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.Today);

    private static (VehiclesController Controller, FakeVehicleService Service) CreateController(bool markAsSoldResult = true)
    {
        var service = new FakeVehicleService { MarkAsSoldResult = markAsSoldResult };
        var controller = new VehiclesController(service, new FakePhotoStorageService(), Options.Create(new PricingOptions()))
        {
            TempData = new TempDataDictionary(new DefaultHttpContext(), new FakeTempDataProvider())
        };
        return (controller, service);
    }

    [Fact]
    public async Task MarkAsSold_WithoutDate_RedirectsToDetailsWithAnError()
    {
        var (controller, service) = CreateController();

        var result = await controller.MarkAsSold(1, saleDate: null);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirect.ActionName);
        Assert.Equal(1, redirect.RouteValues?["id"]);
        Assert.NotNull(controller.TempData["SaleError"]);
        Assert.Null(service.ReceivedSaleDate);
    }

    [Fact]
    public async Task MarkAsSold_WithAFutureDate_RedirectsToDetailsWithAnError()
    {
        var (controller, service) = CreateController();

        var result = await controller.MarkAsSold(1, Today.AddDays(1));

        Assert.IsType<RedirectToActionResult>(result);
        Assert.NotNull(controller.TempData["SaleError"]);
        Assert.Null(service.ReceivedSaleDate);
    }

    [Fact]
    public async Task MarkAsSold_WithAValidDate_PassesItToTheService()
    {
        var (controller, service) = CreateController();
        var saleDate = Today.AddDays(-3);

        var result = await controller.MarkAsSold(1, saleDate);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirect.ActionName);
        Assert.Equal(saleDate, service.ReceivedSaleDate);
        Assert.Null(controller.TempData["SaleError"]);
    }

    [Fact]
    public async Task MarkAsSold_WhenTheServiceRefuses_RedirectsToDetailsWithAnError()
    {
        var (controller, _) = CreateController(markAsSoldResult: false);

        var result = await controller.MarkAsSold(1, Today);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirect.ActionName);
        Assert.NotNull(controller.TempData["SaleError"]);
    }

    private sealed class FakeVehicleService : IVehicleService
    {
        public bool MarkAsSoldResult { get; init; }
        public DateOnly? ReceivedSaleDate { get; private set; }

        public Task<bool> MarkAsSoldAsync(int id, DateOnly saleDate)
        {
            ReceivedSaleDate = saleDate;
            return Task.FromResult(MarkAsSoldResult);
        }

        public Task<IReadOnlyList<Vehicle>> GetInventoryAsync() => throw new NotSupportedException();
        public Task<Vehicle?> GetDetailAsync(int id) => throw new NotSupportedException();
        public Task<Vehicle?> GetForEditAsync(int id) => throw new NotSupportedException();
        public Task<int> AddAsync(Vehicle vehicle) => throw new NotSupportedException();
        public Task<bool> UpdateAsync(Vehicle vehicle) => throw new NotSupportedException();
        public Task<bool> DeleteAsync(int id) => throw new NotSupportedException();
        public Task<int> GetOrCreateSpecificationIdAsync(string brandName, string modelName, string trimName) => throw new NotSupportedException();
        public Task<CatalogueNames> GetCatalogueNamesAsync() => throw new NotSupportedException();
    }

    private sealed class FakePhotoStorageService : IPhotoStorageService
    {
        public string? Validate(IFormFile file) => null;
        public Task<string> SaveAsync(IFormFile file) => throw new NotSupportedException();
        public void Delete(string? photoUrl) { }
    }

    private sealed class FakeTempDataProvider : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();
        public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
    }
}
