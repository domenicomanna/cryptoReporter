namespace Api.Common.Attributes;

/// <summary>
/// Indicates the property name that should be used when sorting
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class SortName : Attribute
{
    public string Name { get; private set; }

    public SortName(string name)
    {
        Name = name;
    }
}
