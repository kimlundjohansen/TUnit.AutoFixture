using System.Reflection;
using AutoFixture;

namespace TUnit.AutoFixture;

/// <summary>
/// Abstract base class for attributes that customize AutoFixture behavior for test parameters.
/// Derive from this class to create custom customization attributes.
/// </summary>
/// <example>
/// <code>
/// [AttributeUsage(AttributeTargets.Parameter)]
/// public class MyCustomAttribute : CustomizeAttribute
/// {
///     public override ICustomization GetCustomization(ParameterInfo parameter)
///     {
///         return new MyCustomCustomization(parameter.ParameterType);
///     }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
public abstract class CustomizeAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the priority of this customization.
    /// Lower values are applied first. Default is 0.
    /// </summary>
    /// <remarks>
    /// Use this property to control the order in which customizations are applied
    /// when multiple customization attributes are present on the same parameter.
    /// </remarks>
    public int Priority { get; set; }

    /// <summary>
    /// Gets the AutoFixture customization to apply for the specified parameter.
    /// </summary>
    /// <param name="parameter">The parameter to customize.</param>
    /// <returns>An <see cref="ICustomization"/> that will be applied to the AutoFixture instance.</returns>
    public abstract ICustomization GetCustomization(ParameterInfo parameter);
}