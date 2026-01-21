using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;

namespace TUnit.AutoFixture.Internal;

/// <summary>
/// Internal customization that implements the freezing logic for the FrozenAttribute.
/// This class is responsible for creating a single instance and reusing it throughout
/// the object graph according to the specified matching strategy.
/// </summary>
internal sealed class FreezeOnMatchCustomization : ICustomization
{
    private readonly Type _targetType;
    private readonly Matching _matching;

    /// <summary>
    /// Initializes a new instance of the <see cref="FreezeOnMatchCustomization"/> class.
    /// </summary>
    /// <param name="targetType">The type to freeze.</param>
    /// <param name="matching">The matching strategy to use.</param>
    public FreezeOnMatchCustomization(Type targetType, Matching matching)
    {
        _targetType = targetType ?? throw new ArgumentNullException(nameof(targetType));
        _matching = matching;
    }

    /// <summary>
    /// Customizes the fixture by freezing the target type according to the matching strategy.
    /// </summary>
    /// <param name="fixture">The fixture to customize.</param>
    public void Customize(IFixture fixture)
    {
        if (fixture is null)
        {
            throw new ArgumentNullException(nameof(fixture));
        }

        // Create a single specimen instance
        var context = new SpecimenContext(fixture);
        var specimen = context.Resolve(_targetType);

        // Freeze for the exact target type
        InjectSpecimen(fixture, _targetType, specimen);

        // Apply additional matching based on the strategy
        switch (_matching)
        {
            case Matching.ExactType:
                // Already frozen above, nothing more to do
                break;

            case Matching.ImplementedInterfaces:
                this.FreezeForImplementedInterfaces(fixture, specimen);
                break;

            case Matching.DirectBaseType:
                this.FreezeForDirectBaseType(fixture, specimen);
                break;

            case Matching.BaseType:
                this.FreezeForAllBaseTypes(fixture, specimen);
                break;

            case Matching.MemberOfFamily:
                this.FreezeForImplementedInterfaces(fixture, specimen);
                this.FreezeForAllBaseTypes(fixture, specimen);
                break;

            default:
                throw new ArgumentOutOfRangeException(
                    "matching",
                    _matching,
                    $"Unknown matching strategy: {_matching}");
        }
    }

    private static void InjectSpecimen(IFixture fixture, Type type, object? specimen)
    {
        // Use AutoFixture's Inject<T> extension method via reflection
        var injectMethod = typeof(FixtureRegistrar)
            .GetMethod(nameof(FixtureRegistrar.Inject), BindingFlags.Static | BindingFlags.Public);

        if (injectMethod is not null)
        {
            var genericMethod = injectMethod.MakeGenericMethod(type);
            genericMethod.Invoke(null, [fixture, specimen]);
        }
    }

    private void FreezeForImplementedInterfaces(IFixture fixture, object specimen)
    {
        foreach (var interfaceType in _targetType.GetInterfaces())
        {
            InjectSpecimen(fixture, interfaceType, specimen);
        }
    }

    private void FreezeForDirectBaseType(IFixture fixture, object specimen)
    {
        var baseType = _targetType.BaseType;
        if (baseType is not null && baseType != typeof(object))
        {
            InjectSpecimen(fixture, baseType, specimen);
        }
    }

    private void FreezeForAllBaseTypes(IFixture fixture, object specimen)
    {
        var baseType = _targetType.BaseType;
        while (baseType is not null && baseType != typeof(object))
        {
            InjectSpecimen(fixture, baseType, specimen);
            baseType = baseType.BaseType;
        }
    }
}