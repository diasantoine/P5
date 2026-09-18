using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using P5.Configuration;
using P5.Data;
using P5.Security;
using P5.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Un compte n'a pas besoin d'être confirmé par email pour se connecter.
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddErrorDescriber<FrenchIdentityErrorDescriber>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Culture fixée explicitement : le binding et le formatage des décimaux sont ainsi indépendants de la culture du système hôte.
var cultureFr = new CultureInfo("fr-FR");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(cultureFr);
    options.SupportedCultures = [cultureFr];
    options.SupportedUICultures = [cultureFr];
});

// MVC rédige en anglais les messages qu'il produit lui-même (champ non numérique, valeur illisible,
// champ obligatoire d'un type valeur). Ils sont vus par l'utilisateur, donc traduits.
builder.Services.AddControllersWithViews(options =>
{
    var messages = options.ModelBindingMessageProvider;
    messages.SetValueMustBeANumberAccessor(field => $"Le champ « {field} » doit être un nombre.");
    messages.SetValueMustNotBeNullAccessor(field => $"Le champ « {field} » est obligatoire.");
    messages.SetAttemptedValueIsInvalidAccessor((value, field) => $"La valeur « {value} » n'est pas valide pour « {field} ».");
    messages.SetValueIsInvalidAccessor(value => $"La valeur « {value} » n'est pas valide.");
    messages.SetMissingBindRequiredValueAccessor(field => $"Le champ « {field} » est obligatoire.");
});
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IRepairService, RepairService>();
builder.Services.AddScoped<IPhotoStorageService, PhotoStorageService>();
builder.Services.Configure<PricingOptions>(builder.Configuration.GetSection(PricingOptions.SectionName));

builder.Services.AddRazorPages();

var app = builder.Build();

// Base de données prête au premier lancement, sans commande à taper :
//  1. les migrations en attente sont appliquées (création de la base comprise) ;
//  2. l'inventaire de départ est inséré, uniquement si la base est vide.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
    await SeedData.SeedAsync(db);
    await IdentitySeed.SeedAdminAsync(scope.ServiceProvider);
}

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRequestLocalization();

// MapStaticAssets ne sert que les fichiers présents au build ; les photos téléversées à l'exécution ont besoin de UseStaticFiles.
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Vehicles}/{action=Index}/{id?}") // la page d'accueil du site est l'inventaire
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
