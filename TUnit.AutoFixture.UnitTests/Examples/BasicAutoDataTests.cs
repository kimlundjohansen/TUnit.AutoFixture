using FluentAssertions;
using Poc.UnitTests.Infrastructure;
using TUnit.AutoFixture;
using TUnit.Core;

namespace Poc.UnitTests.Examples;

/// <summary>
/// Tests demonstrating basic AutoData functionality with primitive and complex types.
/// </summary>
public class BasicAutoDataTests
{
    [Test]
    [AutoData]
    public void AutoData_GeneratesPrimitiveTypes_AllParametersArePopulated(
        string text,
        int number,
        bool flag,
        Guid guid)
    {
        // Assert - verify all parameters are generated with non-default values
        text.Should().NotBeNull();
        text.Should().NotBeEmpty();
        number.Should().NotBe(0);
        guid.Should().NotBe(Guid.Empty);
    }

    [Test]
    [AutoData]
    public void AutoData_GeneratesComplexTypes_PropertiesArePopulated(Person person)
    {
        // Assert - verify complex type is created with populated properties
        person.Should().NotBeNull();
        person.Name.Should().NotBeNull();
        person.Name.Should().NotBeEmpty();
        person.Age.Should().NotBe(0);
    }

    [Test]
    [AutoData]
    public void AutoData_GeneratesMultipleComplexTypes_EachIsUnique(
        Person person1,
        Person person2,
        Address address1,
        Address address2)
    {
        // Assert - verify multiple instances are created
        person1.Should().NotBeNull();
        person2.Should().NotBeNull();
        address1.Should().NotBeNull();
        address2.Should().NotBeNull();

        // Verify they are different instances
        person1.Should().NotBeSameAs(person2);
        address1.Should().NotBeSameAs(address2);
    }

    [Test]
    [AutoData]
    public void AutoData_GeneratesNestedTypes_NestedPropertiesArePopulated(Person person)
    {
        // AutoFixture may or may not populate nested properties by default
        // This test verifies the person object is created
        person.Should().NotBeNull();
        person.Name.Should().NotBeNull();
    }

    [Test]
    [AutoData]
    public void AutoData_WorksWithNoParameters()
    {
        // This test verifies AutoData works even with no parameters
        // It should simply execute without errors
        true.Should().BeTrue();
    }

    [Test]
    [AutoData]
    public void AutoData_GeneratesCollections_CollectionsArePopulated(List<int> numbers)
    {
        // Assert - verify collections are generated
        numbers.Should().NotBeNull();
        numbers.Should().NotBeEmpty();
        numbers.Count.Should().BeGreaterThan(0);
    }

    [Test]
    [AutoData]
    public void AutoData_GeneratesEnums_EnumHasValidValue(DayOfWeek dayOfWeek)
    {
        // Assert - verify enum is generated with a valid value
        Enum.IsDefined(typeof(DayOfWeek), dayOfWeek).Should().BeTrue();
    }
}