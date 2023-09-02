namespace Api.Common.Attributes;

/// <summary>
/// Indicates that the property is not sortable
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class NotSortable : Attribute { }
