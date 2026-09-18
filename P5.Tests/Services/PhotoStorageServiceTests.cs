using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using P5.Services;

namespace P5.Tests.Services;

public sealed class PhotoStorageServiceTests : IDisposable
{
    private static readonly byte[] PngHeader = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

    private readonly string _root = Path.Combine(Path.GetTempPath(), "p5-tests-" + Guid.NewGuid().ToString("N"));
    private readonly PhotoStorageService _service;

    public PhotoStorageServiceTests()
    {
        Directory.CreateDirectory(_root);
        _service = new PhotoStorageService(new FakeEnvironment { WebRootPath = _root });
    }

    public void Dispose() => Directory.Delete(_root, recursive: true);

    private static FormFile CreateFile(string name, byte[] content) =>
        new(new MemoryStream(content), 0, content.Length, "Photo", name);

    [Fact]
    public void Validate_AcceptsARealPng() =>
        Assert.Null(_service.Validate(CreateFile("voiture.png", PngHeader)));

    [Fact]
    public void Validate_RejectsAnUnknownExtension() =>
        Assert.NotNull(_service.Validate(CreateFile("voiture.exe", PngHeader)));

    [Fact]
    public void Validate_RejectsAFileWhoseContentIsNotAnImage() =>
        Assert.NotNull(_service.Validate(CreateFile("voiture.png", "<script>"u8.ToArray())));

    [Fact]
    public void Validate_RejectsAFileOverFiveMegabytes() =>
        Assert.NotNull(_service.Validate(CreateFile("voiture.png", new byte[PhotoStorageService.MaxSizeInBytes + 1])));

    [Fact]
    public async Task SaveAsync_NeverReusesTheNameSentByTheBrowser()
    {
        var url = await _service.SaveAsync(CreateFile(@"..\..\web.config.png", PngHeader));

        Assert.StartsWith("/uploads/vehicles/", url);
        Assert.DoesNotContain("web.config", url);
        Assert.Single(Directory.GetFiles(Path.Combine(_root, "uploads", "vehicles")));
    }

    [Fact]
    public async Task Delete_RemovesAnUploadedPhoto_AndIgnoresForeignAddresses()
    {
        var url = await _service.SaveAsync(CreateFile("voiture.png", PngHeader));
        var outside = Path.Combine(_root, "secret.txt");
        File.WriteAllText(outside, "x");

        _service.Delete("/uploads/vehicles/../../secret.txt");
        _service.Delete("https://exemple.fr/photo.png");
        _service.Delete(url);

        Assert.True(File.Exists(outside));
        Assert.Empty(Directory.GetFiles(Path.Combine(_root, "uploads", "vehicles")));
    }

    private sealed class FakeEnvironment : IWebHostEnvironment
    {
        public string WebRootPath { get; set; } = string.Empty;
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string ApplicationName { get; set; } = "P5.Tests";
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        public string ContentRootPath { get; set; } = string.Empty;
        public string EnvironmentName { get; set; } = "Test";
    }
}
