using System.ComponentModel;
using System.Reflection;
using Api.Common.Attributes;
using Api.Common.ExtensionMethods;

namespace Api.Utils;

public class InvalidSortStatementException : Exception
{
    public InvalidSortStatementException() { }

    public InvalidSortStatementException(string message)
        : base(message) { }
}

public enum SortDirection
{
    [Description("asc")]
    Ascending,

    [Description("desc")]
    Descending
}

public class SortItem
{
    public string PropertyName { get; set; } = string.Empty;
    public SortDirection SortDirection { get; set; }
}

public class SortableProperty
{
    public string PropertyName { get; set; } = string.Empty;
    public string SortName { get; set; } = string.Empty;
}

public class SortStatementGenerator
{
    public static string GenerateSortStatement(string originalSortStatement, Type type)
    {
        if (originalSortStatement.Trim().Length == 0)
        {
            throw new InvalidSortStatementException($"Sort statement must not be empty");
        }
        List<SortItem> sortItems = ParseSortStatement(originalSortStatement);
        List<SortableProperty> sortableProperties = GetSortableProperties(type);
        List<string> sortValues = sortItems
            .Select(sortItem =>
            {
                SortableProperty? sortableProperty = sortableProperties.FirstOrDefault(
                    x => x.PropertyName.ToLower() == sortItem.PropertyName.ToLower()
                );
                if (sortableProperty is null)
                {
                    throw new InvalidSortStatementException($"The property '{sortItem.PropertyName}' is not sortable.");
                }
                return $"{sortableProperty.SortName} {sortItem.SortDirection}";
            })
            .ToList();

        return string.Join(", ", sortValues);
    }

    private static List<SortItem> ParseSortStatement(string sortStatement)
    {
        List<string> valuesSeparatedByCommas = CsvStringToListConverter.ConvertToList(sortStatement);

        List<SortItem> sortItems = valuesSeparatedByCommas
            .Select(value =>
            {
                if (value.IndexOf(" ") == -1)
                    return new SortItem { PropertyName = value, SortDirection = SortDirection.Ascending };

                string[] values = value.Split(" ");
                return new SortItem { PropertyName = values[0], SortDirection = GetSortDirection(values[1]) };
            })
            .ToList();

        return sortItems;
    }

    private static SortDirection GetSortDirection(string sortDirection)
    {
        sortDirection = sortDirection.ToLower();

        if (sortDirection == SortDirection.Ascending.GetDescription())
            return SortDirection.Ascending;

        if (sortDirection == SortDirection.Descending.GetDescription())
            return SortDirection.Descending;

        throw new InvalidSortStatementException(
            $"Sort direction {sortDirection} is invalid. Sort direction must be asc or desc."
        );
    }

    /// <summary>
    /// Returns the sortable properties for the given type
    /// </summary>
    private static List<SortableProperty> GetSortableProperties(Type type)
    {
        List<SortableProperty> sortableProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => !Attribute.IsDefined(property, typeof(NotSortable))) // filter out all properties that are not sortable
            .Select(property =>
            {
                SortName? sortNameAttribute =
                    property.GetCustomAttributes(typeof(SortName), false).FirstOrDefault() as SortName;
                return new SortableProperty
                {
                    PropertyName = property.Name,
                    SortName = sortNameAttribute is null ? property.Name : sortNameAttribute.Name
                };
            })
            .ToList();

        return sortableProperties;
    }
}
