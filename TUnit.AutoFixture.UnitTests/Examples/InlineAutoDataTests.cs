using FluentAssertions;
using Poc.UnitTests.Infrastructure;
using TUnit.AutoFixture;
using TUnit.AutoFixture.NSubstitute;

namespace Poc.UnitTests.Examples;

/// <summary>
/// Tests demonstrating InlineAutoData functionality with hybrid explicit and auto-generated values.
/// </summary>
public class InlineAutoDataTests
{
    [Test]
    [InlineAutoData("explicit string", 42)]
    public void InlineAutoData_CombinesExplicitAndAuto_ValuesAreCorrect(
        string explicitText,
        int explicitNumber,
        Person autoPerson,
        Address autoAddress)
    {
        // Assert - verify explicit values are used
        explicitText.Should().Be("explicit string");
        explicitNumber.Should().Be(42);

        // Assert - verify auto-generated values are created
        autoPerson.Should().NotBeNull();
        autoPerson.Name.Should().NotBeNull();
        autoAddress.Should().NotBeNull();
        autoAddress.Street.Should().NotBeNull();
    }

    [Test]
    [InlineAutoData("John", 25)]
    [InlineAutoData("Jane", 30)]
    [InlineAutoData("Bob", 35)]
    public void InlineAutoData_MultipleAttributes_CreatesMultipleTestCases(
        string name,
        int age,
        Address address)
    {
        // This test runs 3 times, once for each InlineAutoData attribute
        // Verify explicit values are one of the expected values
        name.Should().BeOneOf("John", "Jane", "Bob");
        age.Should().BeOneOf(25, 30, 35);

        // Verify auto-generated value
        address.Should().NotBeNull();
        address.Street.Should().NotBeEmpty();
    }

    [Test]
    [InlineAutoData(100, 200, 300)]
    public void InlineAutoData_AllExplicitValues_NoAutoGeneration(
        int value1,
        int value2,
        int value3)
    {
        // When all parameters have explicit values, no auto-generation occurs
        value1.Should().Be(100);
        value2.Should().Be(200);
        value3.Should().Be(300);
    }

    [Test]
    [InlineAutoData()]
    public void InlineAutoData_NoExplicitValues_BehavesLikeAutoData(
        string text,
        int number,
        Person person)
    {
        // When no explicit values are provided, behaves identical to [AutoData]
        text.Should().NotBeNull();
        number.Should().NotBe(0);
        person.Should().NotBeNull();
    }

    [Test]
    [InlineAutoNSubstituteData("explicit")]
    public void InlineAutoData_WithFrozenParameter_FrozenWorksInAutoGeneration(
        string explicitValue,
        [Frozen] IService frozenService,
        Consumer consumer)
    {
        // Verify explicit value
        explicitValue.Should().Be("explicit");

        // Verify frozen service is injected into consumer
        frozenService.Should().NotBeNull();
        consumer.Should().NotBeNull();
        consumer.Service.Should().BeSameAs(frozenService);
    }

    [Test]
    [InlineAutoData(true, false)]
    [InlineAutoData(false, true)]
    public void InlineAutoData_WithBooleans_PreservesExplicitBooleanValues(
        bool flag1,
        bool flag2,
        string autoText)
    {
        // Verify explicit boolean values create distinct test cases
        flag1.Should().NotBe(flag2);
        autoText.Should().NotBeNull();
    }

    [Test]
    [InlineAutoData(DayOfWeek.Monday)]
    [InlineAutoData(DayOfWeek.Friday)]
    public void InlineAutoData_WithEnums_PreservesExplicitEnumValues(
        DayOfWeek explicitDay,
        DayOfWeek autoDay,
        string autoText)
    {
        // Verify explicit enum value
        explicitDay.Should().BeOneOf(DayOfWeek.Monday, DayOfWeek.Friday);

        // Verify auto-generated enum
        Enum.IsDefined(typeof(DayOfWeek), autoDay).Should().BeTrue();

        // Verify auto-generated text
        autoText.Should().NotBeNull();
    }

    [Test]
    [InlineAutoData("test", 42, true)]
    public void InlineAutoData_MixedTypes_AllTypesHandledCorrectly(
        string explicitText,
        int explicitNumber,
        bool explicitFlag,
        Guid autoGuid,
        Person autoPerson,
        List<int> autoList)
    {
        // Verify explicit values
        explicitText.Should().Be("test");
        explicitNumber.Should().Be(42);
        explicitFlag.Should().BeTrue();

        // Verify auto-generated values
        autoGuid.Should().NotBe(Guid.Empty);
        autoPerson.Should().NotBeNull();
        autoList.Should().NotBeNull();
        autoList.Should().NotBeEmpty();
    }

    [Test]
    [InlineAutoData(new[] { 1, 2, 3 })]
    public void InlineAutoData_WithArrayParameter_ArrayIsPreserved(
        int[] explicitArray,
        string autoText)
    {
        // Verify explicit array
        explicitArray.Should().NotBeNull();
        explicitArray.Length.Should().Be(3);
        explicitArray[0].Should().Be(1);
        explicitArray[1].Should().Be(2);
        explicitArray[2].Should().Be(3);

        // Verify auto-generated value
        autoText.Should().NotBeNull();
    }

    [Test]
    [InlineAutoData(1)]
    [InlineAutoData(2)]
    [InlineAutoData(3)]
    public void InlineAutoData_SingleExplicitValue_RestAreAutoGenerated(
        int explicitId,
        Person person1,
        Person person2,
        Address address)
    {
        // This creates 3 test cases with different IDs
        explicitId.Should().BeOneOf(1, 2, 3);

        // All other parameters are auto-generated independently for each test case
        person1.Should().NotBeNull();
        person2.Should().NotBeNull();
        address.Should().NotBeNull();

        // Verify they are different instances
        person1.Should().NotBeSameAs(person2);
    }

    [Test]
    [InlineAutoData("prefix")]
    public void InlineAutoData_ExplicitPrefixAutoSuffix_CombinationWorks(
        string prefix,
        string autoSuffix)
    {
        // Common pattern: explicit prefix/identifier, auto-generated additional data
        prefix.Should().Be("prefix");
        autoSuffix.Should().NotBeNull();

        // You could concatenate them
        var combined = $"{prefix}_{autoSuffix}";
        combined.Should().StartWith("prefix_");
    }

    [Test]
    [InlineAutoData("derived")]
    public void InlineAutoData_WithInheritedTypes_WorksCorrectly(
        string explicitLabel,
        DerivedClass autoInstance)
    {
        // Verify explicit parameter
        explicitLabel.Should().Be("derived");

        // Verify auto-generated derived class instance
        autoInstance.Should().NotBeNull();
        autoInstance.BaseProperty.Should().NotBeNull();
        autoInstance.DerivedProperty.Should().NotBeNull();
    }
}
