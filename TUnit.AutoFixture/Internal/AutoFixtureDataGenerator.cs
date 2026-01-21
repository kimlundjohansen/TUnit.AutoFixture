using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;
using TUnit.Core;

namespace TUnit.AutoFixture.Internal;

/// <summary>
/// Internal class responsible for generating test parameter values using AutoFixture.
/// This class handles the two-pass algorithm: first applying customizations (like Frozen),
/// then generating specimens for each parameter.
/// </summary>
internal sealed class AutoFixtureDataGenerator
{
    private readonly IFixture fixture;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoFixtureDataGenerator"/> class.
    /// </summary>
    /// <param name="fixture">The AutoFixture instance to use for generation.</param>
    public AutoFixtureDataGenerator(IFixture fixture)
    {
        this.fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
    }

    /// <summary>
    /// Generates values for the specified parameters using AutoFixture.
    /// </summary>
    /// <param name="parameters">The parameters to generate values for.</param>
    /// <returns>An array of generated values corresponding to the parameters.</returns>
    public object?[] Generate(IMemberMetadata[] parameters)
    {
        if (parameters is null)
        {
            throw new ArgumentNullException(nameof(parameters));
        }

        if (parameters.Length == 0)
        {
            return [];
        }

        // Pass 1: Apply customizations (including [Frozen])
        this.ApplyCustomizations(parameters);

        // Pass 2: Generate specimens
        var result = new object?[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            result[i] = this.CreateSpecimen(parameters[i]);
        }

        return result;
    }

    private void ApplyCustomizations(IMemberMetadata[] parameters)
    {
        // Collect all customization attributes from all parameters
        var customizations = new List<(CustomizeAttribute Attribute, ParameterInfo Parameter, int Priority)>();

        foreach (var parameter in parameters)
        {
            // Cast to ParameterMetadata to access properties
            if (parameter is not ParameterMetadata paramMetadata)
            {
                continue;
            }

            // Get the reflection info if available (won't be available in AOT scenarios)
            if (paramMetadata.ReflectionInfo is not ParameterInfo paramInfo)
            {
                continue;
            }

            var attributes = paramInfo.GetCustomAttributes<CustomizeAttribute>().ToArray();
            foreach (var attribute in attributes)
            {
                customizations.Add((attribute, paramInfo, attribute.Priority));
            }
        }

        // Sort by priority (lower values first)
        customizations.Sort((a, b) => a.Priority.CompareTo(b.Priority));

        // Apply customizations in priority order
        foreach (var (attribute, parameter, _) in customizations)
        {
            var customization = attribute.GetCustomization(parameter);
            this.fixture.Customize(customization);
        }
    }

    private object? CreateSpecimen(IMemberMetadata parameter)
    {
        var context = new SpecimenContext(this.fixture);

        // Cast to ParameterMetadata to access properties
        if (parameter is ParameterMetadata paramMetadata)
        {
            // If we have reflection info, use it for better compatibility
            if (paramMetadata.ReflectionInfo is not null)
            {
                return context.Resolve(paramMetadata.ReflectionInfo);
            }

            // Otherwise, fall back to resolving by type
            return context.Resolve(paramMetadata.Type);
        }

        // Fallback for other member metadata types
        throw new NotSupportedException($"Member metadata type {parameter.GetType().Name} is not supported.");
    }
}