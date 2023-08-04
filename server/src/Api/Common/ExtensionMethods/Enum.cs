using System.ComponentModel;
using System.Reflection;

namespace Api.Common.ExtensionMethods;

public static class EnumExtensionMethods
{
    public static string GetDescription(this Enum value)
    {
        Type type = value.GetType();
        string? name = Enum.GetName(type, value);
        if (name is null)
            return value.ToString();

        FieldInfo? field = type.GetField(name);
        if (field is null)
            return value.ToString();

        DescriptionAttribute? descriptionAttribute =
            Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
        return descriptionAttribute?.Description ?? value.ToString();
    }
}
