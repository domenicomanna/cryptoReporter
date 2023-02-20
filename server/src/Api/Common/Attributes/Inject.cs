namespace Api.Common.Attributes;

/// <summary>
/// Indicates that the class should be added as a service so that it is available to use through dependency injection
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class Inject : Attribute { }
