using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using P5.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Culture unique et explicite : sans cela, le binding d'un decimal saisi "1800,50"
// et le formatage des montants dependent de la culture du systeme hote.
var cultureFr = new CultureInfo("fr-FR");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(cultureFr);
    options.SupportedCultures = [cultureFr];
    options.SupportedUICultures = [cultureFr];
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Base de données prête au premier lancement, sans commande à taper :
//  1. les migrations en attente sont appliquées (création de la base comprise) ;
//  2. l'inventaire de départ est inséré, uniquement si la base est vide.
// Le DbContext est "scoped" : hors requête HTTP, il faut ouvrir un scope soi-même.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
    await SeedData.SeedAsync(db);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRequestLocalization();

// MapStaticAssets (plus bas) ne sert que les fichiers presents au build. Les photos
// televersees a l'execution dans wwwroot/images ont besoin de UseStaticFiles.
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
