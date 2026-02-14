using Api.Common.Attributes;
using Api.Common.ExtensionMethods.ValidationRules;
using FluentValidation;
using FluentValidation.TestHelper;

namespace ApiTests.Common.ExtensionMethods.ValidationRules;

public class House
{
    public string Street { get; set; } = string.Empty;

    [NotSortable]
    public decimal Price { get; set; }
}

public class GetHousesRequest
{
    public string SortBy { get; set; } = "Street asc";
}

public class GetHousesRequestValidator : AbstractValidator<GetHousesRequest>
{
    public GetHousesRequestValidator()
    {
        RuleFor(x => x.SortBy).SortMustBeValid(typeof(House));
    }
}

public class SortStatementValidatorTests
{
    [Fact]
    public void ThereShouldBeAValidationErrorIfTheSortIsInvalid()
    {
        GetHousesRequestValidator validator = new GetHousesRequestValidator();
        GetHousesRequest request = new GetHousesRequest { SortBy = "Xyz" };
        var result = validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.SortBy);
    }

    [Fact]
    public void ThereShouldNotBeAValidationErrorIfTheSortIsValid()
    {
        GetHousesRequestValidator validator = new GetHousesRequestValidator();
        GetHousesRequest request = new GetHousesRequest { SortBy = "Street" };
        var result = validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.SortBy);
    }
}
