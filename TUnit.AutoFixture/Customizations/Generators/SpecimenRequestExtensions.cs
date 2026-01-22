using System.Reflection;

namespace TUnit.AutoFixture.Customizations.Generators;

/// <summary>
/// Extension methods for specimen requests.
/// </summary>
internal static class SpecimenRequestExtensions
{
    /// <summary>
    /// Determines if an ISpecimenBuilder request is for a specific type.
    /// </summary>
    /// <typeparam name="T">The type to check for.</typeparam>
    /// <param name="request">The specimen request to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the request is for type <typeparamref name="T"/>; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsRequestFor<T>(this object request) => request switch
    {
        ParameterInfo pi => pi.ParameterType == typeof(T),
        Type type => type == typeof(T),
        _ => false,
    };
}
