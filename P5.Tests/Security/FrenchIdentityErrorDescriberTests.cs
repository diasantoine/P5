using Microsoft.AspNetCore.Identity;
using P5.Security;

namespace P5.Tests.Security;

// Identity rédige ses messages en anglais. La règle du projet : identifiants en anglais,
// texte vu par l'utilisateur en français. Ces messages-là sont vus à l'inscription.
public class FrenchIdentityErrorDescriberTests
{
    private readonly IdentityErrorDescriber _describer = new FrenchIdentityErrorDescriber();

    [Fact]
    public void DuplicateEmail_IsWrittenInFrench() =>
        Assert.Equal("Cette adresse email est déjà utilisée.", _describer.DuplicateEmail("client@exemple.fr").Description);

    [Fact]
    public void PasswordTooShort_StatesTheRequiredLength() =>
        Assert.Contains("6 caractères", _describer.PasswordTooShort(6).Description);

    [Theory]
    [InlineData("PasswordRequiresDigit")]
    [InlineData("PasswordRequiresLower")]
    [InlineData("PasswordRequiresUpper")]
    [InlineData("PasswordRequiresNonAlphanumeric")]
    [InlineData("InvalidEmail")]
    [InlineData("DuplicateUserName")]
    public void EveryOverriddenMessage_IsFreeOfEnglish(string methodName)
    {
        var method = typeof(FrenchIdentityErrorDescriber).GetMethod(methodName)!;
        var arguments = method.GetParameters().Length == 0 ? null : new object?[] { "valeur" };
        var error = (IdentityError)method.Invoke(_describer, arguments)!;

        Assert.DoesNotContain(" is ", error.Description);
        Assert.DoesNotContain("must", error.Description);
        Assert.DoesNotContain("password", error.Description, StringComparison.OrdinalIgnoreCase);
    }
}
