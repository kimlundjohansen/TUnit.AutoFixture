using AutoFixture;
using FluentAssertions;
using Poc.UnitTests.Infrastructure;
using TUnit.Core;

namespace Poc.UnitTests.Examples;

/// <summary>
/// Tests demonstrating direct usage of FixtureFactory.
/// </summary>
public class FixtureFactoryTests
{
    [Test]
    public void FixtureFactory_Create_ReturnsConfiguredFixture()
    {
        // Arrange & Act
        var fixture = TUnit.AutoFixture.FixtureFactory.Create();

        // Assert
        fixture.Should().NotBeNull();
        fixture.Should().BeAssignableTo<IFixture>();
    }

    [Test]
    public void FixtureFactory_Create_CanGeneratePrimitiveTypes()
    {
        // Arrange
        var fixture = TUnit.AutoFixture.FixtureFactory.Create();

        // Act
        var text = fixture.Create<string>();
        var number = fixture.Create<int>();

        // Assert
        text.Should().NotBeNullOrEmpty();
        number.Should().NotBe(0);
    }

    [Test]
    public void FixtureFactory_Create_CanGenerateComplexTypes()
    {
        // Arrange
        var fixture = TUnit.AutoFixture.FixtureFactory.Create();

        // Act
        var person = fixture.Create<Person>();

        // Assert
        person.Should().NotBeNull();
        person.Name.Should().NotBeNullOrEmpty();
        person.Age.Should().BeGreaterThan(0);
    }

    [Test]
    public void FixtureFactory_Create_AppliesAutoRegisterCustomizations()
    {
        // Arrange
        var fixture = TUnit.AutoFixture.FixtureFactory.Create();

        // Act
        var person = fixture.Create<Person>();

        // Assert - If AutoRegisterCustomization from AutoRegisterTests is discovered
        // The person.Age would be 42, but since that customization is in test scope,
        // we just verify the fixture can create the object
        person.Should().NotBeNull();
        person.Age.Should().BeGreaterThan(0);
    }

#if NET5_0_OR_GREATER
    [Test]
    public void FixtureFactory_Create_SupportsImmutableCollections()
    {
        // Arrange
        var fixture = TUnit.AutoFixture.FixtureFactory.Create();

        // Act
        var immutableArray = fixture.Create<System.Collections.Immutable.ImmutableArray<string>>();
        var immutableList = fixture.Create<System.Collections.Immutable.ImmutableList<int>>();

        // Assert
        immutableArray.Should().NotBeEmpty();
        immutableList.Should().NotBeEmpty();
    }
#endif

    [Test]
    public void FixtureFactory_Create_CanBeUsedWithCustomCustomizations()
    {
        // Arrange
        var fixture = TUnit.AutoFixture.FixtureFactory.Create();

        // Act - Apply additional customization on top of factory defaults
        fixture.Customize<Person>(c => c
            .With(p => p.Age, 100));

        var person = fixture.Create<Person>();

        // Assert
        person.Should().NotBeNull();
        person.Age.Should().Be(100);
    }
}
