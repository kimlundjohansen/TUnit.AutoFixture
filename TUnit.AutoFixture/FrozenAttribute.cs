using System.Reflection;
using AutoFixture;
using TUnit.AutoFixture.Internal;

namespace TUnit.AutoFixture;

/// <summary>
/// Marks a test parameter to be "frozen" in AutoFixture, meaning the same instance
/// will be reused throughout the object graph construction.
/// This is particularly useful for dependency injection scenarios where you want
/// the same mock or stub to be injected into multiple dependencies.
/// </summary>
/// <example>
/// Basic usage with exact type matching:
/// <code>
/// [Test]
/// [AutoData]
/// public void Test([Frozen] IService service, Consumer consumer)
/// {
///     // The same IService instance is injected into both service parameter
///     // and into consumer's constructor (if Consumer depends on IService)
///     Assert.AreSame(service, consumer.Service);
/// }
/// </code>
/// </example>
/// <example>
/// Advanced usage with interface matching:
/// <code>
/// [Test]
/// [AutoData]
/// public void Test(
///     [Frozen(Matching.ImplementedInterfaces)] ConcreteService service,
///     Consumer consumer)
/// {
///     // ConcreteService is frozen for all interfaces it implements
///     // So if Consumer depends on IService (implemented by ConcreteService),
///     // it will receive the same instance
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class FrozenAttribute : CustomizeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FrozenAttribute"/> class
    /// with exact type matching (default behavior).
    /// </summary>
    public FrozenAttribute()
        : this(Matching.ExactType)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FrozenAttribute"/> class
    /// with the specified matching strategy.
    /// </summary>
    /// <param name="matching">
    /// The matching strategy that determines how the frozen instance
    /// should be matched when injecting into dependencies.
    /// </param>
    public FrozenAttribute(Matching matching)
    {
        Matching = matching;
    }

    /// <summary>
    /// Gets the matching strategy for this frozen parameter.
    /// </summary>
    public Matching Matching { get; }

    /// <summary>
    /// Gets the AutoFixture customization that implements the freezing behavior.
    /// </summary>
    /// <param name="parameter">The parameter to freeze.</param>
    /// <returns>A customization that freezes the parameter according to the matching strategy.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="parameter"/> is null.</exception>
    public override ICustomization GetCustomization(ParameterInfo parameter)
    {
        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        return new Internal.FreezeOnMatchCustomization(parameter.ParameterType, Matching);
    }
}