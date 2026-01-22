#if NET6_0_OR_GREATER
using AutoFixture.Kernel;

namespace TUnit.AutoFixture.Customizations.Generators;

/// <summary>
/// Responsible for generating <see cref="DateOnly"/> instances.
/// </summary>
/// <remarks>
/// This generator is only available in .NET 6.0 and later, where the <see cref="DateOnly"/> type was introduced.
/// </remarks>
[AutoRegister]
public class DateOnlyGenerator : ISpecimenBuilder
{
    /// <summary>
    /// Creates a <see cref="DateOnly"/> instance from a generated <see cref="DateTime"/>.
    /// </summary>
    /// <param name="request">The request that describes what to create.</param>
    /// <param name="context">The context for the current specimen creation operation.</param>
    /// <returns>
    /// A <see cref="DateOnly"/> instance if the request is for a DateOnly;
    /// otherwise, a <see cref="NoSpecimen"/> instance.
    /// </returns>
    public object Create(object request, ISpecimenContext context)
    {
        if (!request.IsRequestFor<DateOnly>())
        {
            return new NoSpecimen();
        }

        var dateTime = (DateTime)context.Resolve(typeof(DateTime));
        return DateOnly.FromDateTime(dateTime);
    }
}
#endif
