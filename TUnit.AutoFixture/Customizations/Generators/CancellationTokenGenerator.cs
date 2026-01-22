using AutoFixture.Kernel;

namespace TUnit.AutoFixture.Customizations.Generators;

/// <summary>
/// Responsible for generating <see cref="CancellationToken"/> instances
/// that have not been canceled.
/// </summary>
[AutoRegister]
public class CancellationTokenGenerator : ISpecimenBuilder
{
    /// <summary>
    /// Creates a non-canceled <see cref="CancellationToken"/> instance.
    /// </summary>
    /// <param name="request">The request that describes what to create.</param>
    /// <param name="context">The context for the current specimen creation operation.</param>
    /// <returns>
    /// A non-canceled <see cref="CancellationToken"/> if the request is for a CancellationToken;
    /// otherwise, a <see cref="NoSpecimen"/> instance.
    /// </returns>
    public object Create(object request, ISpecimenContext context)
    {
        if (!request.IsRequestFor<CancellationToken>())
        {
            return new NoSpecimen();
        }

        return new CancellationToken(canceled: false);
    }
}
