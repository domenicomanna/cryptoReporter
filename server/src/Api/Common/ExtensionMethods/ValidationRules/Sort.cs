using Api.Utils;
using FluentValidation;

namespace Api.Common.ExtensionMethods.ValidationRules;

public static class SortExtension
{
    public static IRuleBuilderOptionsConditions<T, string> SortMustBeValid<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        Type type
    )
    {
        return ruleBuilder.Custom(
            (sortBy, context) =>
            {
                try
                {
                    SortStatementGenerator.GenerateSortStatement(sortBy, type);
                }
                catch (InvalidSortStatementException e)
                {
                    context.AddFailure(e.Message);
                }
                catch (Exception)
                {
                    context.AddFailure("Sort statement could not be parsed");
                }
            }
        );
    }
}
