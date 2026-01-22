using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;

namespace TUnit.AutoFixture.Customizations;

/// <summary>
/// Automatically discovers and registers all customizations marked with <see cref="AutoRegisterAttribute" />.
/// </summary>
/// <remarks>
/// <para>
/// This customization scans all loaded assemblies for types decorated with [AutoRegister]
/// and automatically registers them with the fixture. This eliminates the need to manually
/// register customizations in each test attribute.
/// </para>
/// <para>
/// Supported types:
/// - Classes implementing <see cref="ICustomization"/> are registered via fixture.Customize()
/// - Classes implementing <see cref="ISpecimenBuilder"/> are added to fixture.Customizations
/// </para>
/// <para>
/// The discovery process is cached after the first execution for performance.
/// </para>
/// </remarks>
public class AutoRegisterCustomization : ICustomization
{
    private static readonly Lazy<Type[]> AutoRegisterTypesLazy = new (
        () => AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(GetLoadableTypes)
            .Where(HasAutoRegisterAttribute)
            .ToArray(),
        LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>
    /// Customizes the specified fixture by discovering and registering all [AutoRegister] marked types.
    /// </summary>
    /// <param name="fixture">The fixture to customize.</param>
    /// <exception cref="ArgumentNullException">Thrown when fixture is null.</exception>
    /// <exception cref="NotSupportedException">
    /// Thrown when an [AutoRegister] marked type doesn't implement ICustomization or ISpecimenBuilder.
    /// </exception>
    public void Customize(IFixture fixture)
    {
        if (fixture is null)
        {
            throw new ArgumentNullException(nameof(fixture));
        }

        var autoRegisterTypes = AutoRegisterTypesLazy.Value;

        foreach (var type in autoRegisterTypes)
        {
            var customization = Activator.CreateInstance(type);
            switch (customization)
            {
                case ICustomization c:
                    fixture.Customize(c);
                    break;

                case ISpecimenBuilder b:
                    fixture.Customizations.Add(b);
                    break;

                default:
                    throw new NotSupportedException(
                        $"Invalid type {type.Name}. Only ICustomization and " +
                        $"ISpecimenBuilder is supported for the " +
                        $"AutoRegisterAttribute.");
            }
        }
    }

    private static bool HasAutoRegisterAttribute(Type type)
        => type.GetCustomAttributes(
            typeof(AutoRegisterAttribute),
            inherit: false).Length > 0;

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException e)
        {
            return e.Types?.OfType<Type>()
                   ?? Enumerable.Empty<Type>();
        }
    }
}
