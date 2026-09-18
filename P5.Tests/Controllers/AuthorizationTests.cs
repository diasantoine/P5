using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using P5.Controllers;
using P5.Security;

namespace P5.Tests.Controllers;

// Vérifie que les actions de modification exigent le rôle Admin, et que seules Index et Details restent publiques.
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
