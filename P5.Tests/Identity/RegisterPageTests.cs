using Microsoft.AspNetCore.Mvc;
using P5.Areas.Identity.Pages.Account;

namespace P5.Tests.Identity;

// Vérifie que la page d'inscription est fermée : le site n'a qu'un compte, celui du gérant.
public class RegisterPageTests
{
    [Fact]
    public void OnGet_ReturnsNotFound() =>
        Assert.IsType<NotFoundResult>(new RegisterModel().OnGet());

    [Fact]
    public void OnPost_ReturnsNotFound() =>
        Assert.IsType<NotFoundResult>(new RegisterModel().OnPost());
}
