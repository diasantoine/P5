using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using P5.Controllers;
using P5.Security;

namespace P5.Tests.Controllers;

// L'inscription est ouverte : [Authorize] seul laisserait n'importe quel inscrit modifier l'inventaire.
// Ces tests verrouillent l'exigence du client, « je dois être le seul à pouvoir apporter des modifications ».
public class AuthorizationTests
{
    [Theory]
    [InlineData(typeof(VehiclesController))]
    [InlineData(typeof(RepairsController))]
    public void Controller_RequiresAdminRole(Type controller)
    {
        var authorize = controller.GetCustomAttribute<AuthorizeAttribute>();

        Assert.NotNull(authorize);
        Assert.Equal(AppRoles.Admin, authorize.Roles);
    }

    [Fact]
    public void VehiclesController_OnlyIndexAndDetailsArePublic()
    {
        var publicActions = typeof(VehiclesController)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(m => m.GetCustomAttribute<AllowAnonymousAttribute>() is not null)
            .Select(m => m.Name)
            .Order();

        Assert.Equal(["Details", "Index"], publicActions);
    }

    [Fact]
    public void RepairsController_HasNoPublicAction()
    {
        var publicActions = typeof(RepairsController)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(m => m.GetCustomAttribute<AllowAnonymousAttribute>() is not null);

        Assert.Empty(publicActions);
    }
}
