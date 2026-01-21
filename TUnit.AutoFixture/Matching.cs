namespace TUnit.AutoFixture;

/// <summary>
/// Specifies how a frozen parameter should be matched when injecting into dependencies.
/// Used with the <see cref="FrozenAttribute"/> to control dependency resolution.
/// </summary>
/// <example>
/// <code>
/// [Test]
/// [AutoData]
/// public void Test([Frozen(Matching.ImplementedInterfaces)] IService service, Consumer consumer)
/// {
///     // The same IService instance is injected into both parameters
/// }
/// </code>
/// </example>
public enum Matching
{
    /// <summary>
    /// Matches only the exact type of the frozen parameter.
    /// This is the default matching strategy.
    /// </summary>
    /// <example>
    /// If the parameter type is ConcreteService, only ConcreteService will be frozen.
    /// </example>
    ExactType,

    /// <summary>
    /// Matches all interfaces implemented by the frozen parameter's type.
    /// </summary>
    /// <example>
    /// If the parameter type is ConcreteService implementing IService and IDisposable,
    /// all three types will share the same frozen instance.
    /// </example>
    ImplementedInterfaces,

    /// <summary>
    /// Matches the direct base type of the frozen parameter's type (excluding object).
    /// </summary>
    /// <example>
    /// If the parameter type is DerivedService : BaseService,
    /// both DerivedService and BaseService will share the same frozen instance.
    /// </example>
    DirectBaseType,

    /// <summary>
    /// Matches all base types in the inheritance hierarchy (excluding object).
    /// </summary>
    /// <example>
    /// If the parameter type is DerivedService : MiddleService : BaseService,
    /// all three types will share the same frozen instance.
    /// </example>
    BaseType,

    /// <summary>
    /// Matches the entire type family: all implemented interfaces and all base types.
    /// This is the most inclusive matching strategy.
    /// </summary>
    /// <example>
    /// Freezes the parameter for its concrete type, all implemented interfaces,
    /// and all base types in the inheritance hierarchy.
    /// </example>
    MemberOfFamily
}