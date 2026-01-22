using System.Collections.Immutable;
using AutoFixture;
using FluentAssertions;
using TUnit.AutoFixture.Customizations;
using TUnit.Core;

namespace TUnit.AutoFixture.UnitTests.Examples;

public class AutoRegisterTests
{
    [Test]
    [AutoData]
    public void AutoRegister_ImmutableCollectionsAreAutomaticallySupported_WithoutManualRegistration(
        ImmutableArray<string> immutableArray)
    {
        // ImmutableObjectCustomization is marked with [AutoRegister]
        // and is automatically applied, so immutable collections work
        immutableArray.Should().NotBeEmpty();
    }

    [Test]
    [AutoData]
    public void AutoRegister_CustomTestCustomizationIsApplied_AutomaticallyRegistered(Person person)
    {
        // CustomTestCustomization is marked with [AutoRegister]
        // and sets all Person ages to 42
        person.Age.Should().Be(42);
    }

    [Test]
    [AutoData]
    public void AutoRegister_MultipleCustomizationsWork_AllApplied(
        Person person,
        ImmutableList<int> numbers)
    {
        // Both CustomTestCustomization and ImmutableObjectCustomization
        // are marked with [AutoRegister] and both work
        person.Age.Should().Be(42);
        numbers.Should().NotBeEmpty();
    }

    public class Person
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }
}

/// <summary>
/// A test customization that demonstrates AutoRegister functionality.
/// This customization is automatically discovered and applied to all fixtures.
/// </summary>
[AutoRegister]
public class CustomTestCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        // Customize all Person instances to have Age = 42
        fixture.Customize<AutoRegisterTests.Person>(c => c
            .With(p => p.Age, 42));
    }
}
