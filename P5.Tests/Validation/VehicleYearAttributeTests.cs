using System.ComponentModel.DataAnnotations;
using P5.Validation;

namespace P5.Tests.Validation;

public class VehicleYearAttributeTests
{
    private static ValidationResult? Validate(object? value)
    {
        var attribute = new VehicleYearAttribute();
        var context = new ValidationContext(new object()) { MemberName = "Year" };
        return attribute.GetValidationResult(value, context);
    }

    [Theory]
    [InlineData(1990)]
    [InlineData(2019)]
    public void AcceptsPlausibleYears(int year)
    {
        Assert.Equal(ValidationResult.Success, Validate(year));
    }

    [Fact]
    public void AcceptsTheUpcomingModelYear()
    {
        Assert.Equal(ValidationResult.Success, Validate(DateTime.Today.Year + 1));
    }

    [Theory]
    [InlineData(1989)]
    [InlineData(2117)]
    public void RejectsYearsOutOfBounds(int year)
    {
        Assert.NotEqual(ValidationResult.Success, Validate(year));
    }

    [Fact]
    public void IgnoresNullValueLeftToRequired()
    {
        Assert.Equal(ValidationResult.Success, Validate(null));
    }
}
