using AutoFixture;
using AutoFixture.AutoNSubstitute;
using TUnit.AutoFixture.Customizations;

namespace TUnit.AutoFixture;

/// <summary>
/// Factory for creating <see cref="IFixture"/> instances with default customizations.
/// </summary>
/// <remarks>
/// <para>
/// This factory provides a centralized way to create AutoFixture instances with
/// consistent default customizations applied across all tests.
/// </para>
/// <para>
/// Default customizations include:
/// - AutoRegisterCustomization - Automatically discovers and applies [AutoRegister] marked customizations
/// - This includes ImmutableObjectCustomization for .NET 5.0+ (automatically discovered)
/// </para>
/// <para>
/// For NSubstitute auto-mocking support, use <see cref="CreateWithNSubstitute"/> which additionally applies:
/// - AutoNSubstituteCustomization - Automatically mocks interfaces and abstract classes
/// </para>
/// </remarks>
public static class FixtureFactory
{
    /// <summary>
    /// Creates an <see cref="IFixture"/> instance with default customizations.
    /// </summary>
    /// <returns>
    /// A configured <see cref="IFixture"/> instance with AutoRegisterCustomization applied.
    /// </returns>
    /// <remarks>
    /// This method creates a new Fixture instance and applies AutoRegisterCustomization,
    /// which automatically discovers and registers all customizations marked with [AutoRegister].
    /// </remarks>
    /// <example>
    /// Using the factory directly:
    /// <code>
    /// var fixture = FixtureFactory.Create();
    /// var person = fixture.Create&lt;Person&gt;();
    /// </code>
    /// </example>
    public static IFixture Create()
    {
        return new Fixture()
            .Customize(new AutoRegisterCustomization());
    }

    /// <summary>
    /// Creates an <see cref="IFixture"/> instance with NSubstitute auto-mocking and default customizations.
    /// </summary>
    /// <returns>
    /// A configured <see cref="IFixture"/> instance with AutoNSubstituteCustomization and
    /// AutoRegisterCustomization applied.
    /// </returns>
    /// <remarks>
    /// This method creates a new Fixture instance and applies:
    /// 1. AutoNSubstituteCustomization with ConfigureMembers = false
    /// 2. AutoRegisterCustomization for automatic discovery of [AutoRegister] customizations
    /// </remarks>
    /// <example>
    /// Using the factory directly:
    /// <code>
    /// var fixture = FixtureFactory.CreateWithNSubstitute();
    /// var mockService = fixture.Create&lt;IService&gt;(); // Automatically mocked
    /// var person = fixture.Create&lt;Person&gt;();
    /// </code>
    /// </example>
    public static IFixture CreateWithNSubstitute()
    {
        return new Fixture()
            .Customize(new AutoNSubstituteCustomization
            {
                // Configure all mocks to use CallBase = false by default
                // This means mocks won't try to call base class implementations
                ConfigureMembers = false,
            })
            .Customize(new AutoRegisterCustomization());
    }
}
