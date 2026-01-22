using AutoFixture;
using FluentAssertions;
using NSubstitute;
using Poc.UnitTests.Infrastructure;
using TUnit.Core;

namespace Poc.UnitTests.Examples;

/// <summary>
/// Tests demonstrating direct usage of NSubstitute FixtureFactory.
/// </summary>
public class NSubstituteFixtureFactoryTests
{
    [Test]
    public void NSubstituteFixtureFactory_Create_ReturnsConfiguredFixture()
    {
        // Arrange & Act
        var fixture = TUnit.AutoFixture.FixtureFactory.CreateWithNSubstitute();

        // Assert
        fixture.Should().NotBeNull();
        fixture.Should().BeAssignableTo<IFixture>();
    }

    [Test]
    public void NSubstituteFixtureFactory_Create_AutomaticallyMocksInterfaces()
    {
        // Arrange
        var fixture = TUnit.AutoFixture.FixtureFactory.CreateWithNSubstitute();

        // Act
        var service = fixture.Create<IService>();

        // Assert
        service.Should().NotBeNull();
        service.Should().BeAssignableTo<IService>();
    }

    [Test]
    public void NSubstituteFixtureFactory_Create_MocksCanBeConfigured()
    {
        // Arrange
        var fixture = TUnit.AutoFixture.FixtureFactory.CreateWithNSubstitute();
        var service = fixture.Create<IService>();

        // Act
        service.GetData().Returns("test data");
        var result = service.GetData();

        // Assert
        result.Should().Be("test data");
    }

    [Test]
    public void NSubstituteFixtureFactory_Create_MocksCanBeVerified()
    {
        // Arrange
        var fixture = TUnit.AutoFixture.FixtureFactory.CreateWithNSubstitute();
        var service = fixture.Create<IService>();

        // Act
        service.GetData();

        // Assert
        service.Received(1).GetData();
    }

    [Test]
    public void NSubstituteFixtureFactory_Create_CanGenerateComplexTypesWithMockedDependencies()
    {
        // Arrange
        var fixture = TUnit.AutoFixture.FixtureFactory.CreateWithNSubstitute();

        // Act
        var consumer = fixture.Create<Consumer>();

        // Assert
        consumer.Should().NotBeNull();
        consumer.Service.Should().NotBeNull();
        consumer.Service.Should().BeAssignableTo<IService>();
    }

    [Test]
    public void NSubstituteFixtureFactory_Create_AppliesAutoRegisterCustomizations()
    {
        // Arrange
        var fixture = TUnit.AutoFixture.FixtureFactory.CreateWithNSubstitute();

        // Act
        var person = fixture.Create<Person>();

        // Assert - Verify AutoRegisterCustomization is applied
        person.Should().NotBeNull();
        person.Age.Should().BeGreaterThan(0);
    }

#if NET5_0_OR_GREATER
    [Test]
    public void NSubstituteFixtureFactory_Create_SupportsImmutableCollections()
    {
        // Arrange
        var fixture = TUnit.AutoFixture.FixtureFactory.CreateWithNSubstitute();

        // Act
        var immutableArray = fixture.Create<System.Collections.Immutable.ImmutableArray<string>>();
        var immutableList = fixture.Create<System.Collections.Immutable.ImmutableList<int>>();

        // Assert
        immutableArray.Should().NotBeEmpty();
        immutableList.Should().NotBeEmpty();
    }
#endif

    [Test]
    public void NSubstituteFixtureFactory_Create_CanFreezeInstances()
    {
        // Arrange
        var fixture = TUnit.AutoFixture.FixtureFactory.CreateWithNSubstitute();

        // Act - Freeze a mock service
        var frozenService = fixture.Freeze<IService>();
        var consumer = fixture.Create<Consumer>();

        // Assert - The frozen service should be injected into the consumer
        consumer.Service.Should().BeSameAs(frozenService);
    }
}
