#if NET5_0_OR_GREATER
using System.Collections.Immutable;
using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;

namespace TUnit.AutoFixture.Customizations;

/// <summary>
/// Registers customizations with AutoFixture to automatically generate immutable collection instances.
/// </summary>
/// <remarks>
/// This customization enables AutoFixture to create instances of immutable collection types
/// such as ImmutableArray, ImmutableList, ImmutableDictionary, ImmutableHashSet, ImmutableSortedSet,
/// and ImmutableSortedDictionary by first generating their mutable counterparts and then converting them.
/// </remarks>
[AutoRegister]
public class ImmutableObjectCustomization : ICustomization
{
    /// <summary>
    /// Customizes the specified fixture by adding builders for immutable collection types.
    /// </summary>
    /// <param name="fixture">The fixture to customize.</param>
    public void Customize(IFixture fixture)
    {
        fixture.Customizations.Add(
            new ImmutableObjectBuilder(
                typeof(ImmutableArray<>),
                typeof(List<>),
                o => ImmutableArray.ToImmutableArray(o)));

        fixture.Customizations.Add(
            new ImmutableObjectBuilder(
                typeof(ImmutableList<>),
                typeof(List<>),
                o => ImmutableList.ToImmutableList(o)));

        fixture.Customizations.Add(
            new ImmutableObjectBuilder(
                typeof(ImmutableDictionary<,>),
                typeof(Dictionary<,>),
                o => ImmutableDictionary.ToImmutableDictionary(o)));

        fixture.Customizations.Add(
            new ImmutableObjectBuilder(
                typeof(ImmutableHashSet<>),
                typeof(HashSet<>),
                o => ImmutableHashSet.ToImmutableHashSet(o)));

        fixture.Customizations.Add(
            new ImmutableObjectBuilder(
                typeof(ImmutableSortedSet<>),
                typeof(SortedSet<>),
                o => ImmutableSortedSet.ToImmutableSortedSet(o)));

        fixture.Customizations.Add(
            new ImmutableObjectBuilder(
                typeof(ImmutableSortedDictionary<,>),
                typeof(SortedDictionary<,>),
                o => ImmutableSortedDictionary.ToImmutableSortedDictionary(o)));
    }

    private sealed class ImmutableObjectBuilder(
        Type immutableType,
        Type underlyingType,
        Func<dynamic, object> converter)
        : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (GetRequestType(request) is { } type
                && type.IsGenericType
                && type.GetGenericTypeDefinition() == immutableType
                && type.GetGenericArguments() is { Length: > 0 } args)
            {
                var listType = underlyingType.MakeGenericType(args);
                dynamic list = context.Resolve(listType);

                return converter.Invoke(list);
            }

            return new NoSpecimen();
        }

        private static Type? GetRequestType(object request)
            => request switch
            {
                ParameterInfo pi => pi.ParameterType,
                Type t => t,
                _ => null,
            };
    }
}
#endif
