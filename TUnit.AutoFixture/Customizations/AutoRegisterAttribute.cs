namespace TUnit.AutoFixture.Customizations;

/// <summary>
/// Marks a customization class to be automatically registered with AutoFixture.
/// Classes marked with this attribute must implement either ICustomization
/// or ISpecimenBuilder.
/// </summary>
/// <remarks>
/// <para>
/// This attribute enables automatic discovery and registration of customizations across assemblies.
/// When AutoDataAttribute is used, all types marked with [AutoRegister] are automatically
/// discovered and registered with the fixture instance.
/// </para>
/// <para>
/// Example:
/// <code>
/// [AutoRegister]
/// public class MyCustomCustomization : ICustomization
/// {
///     public void Customize(IFixture fixture)
///     {
///         // Your customization logic
///     }
/// }
/// </code>
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class AutoRegisterAttribute : Attribute
{
}
