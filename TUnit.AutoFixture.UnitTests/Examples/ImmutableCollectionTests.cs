#if NET5_0_OR_GREATER
using System.Collections.Immutable;
using FluentAssertions;
using TUnit.Core;

namespace TUnit.AutoFixture.UnitTests.Examples;

public class ImmutableCollectionTests
{
    [Test]
    [AutoData]
    public void AutoData_GeneratesImmutableArray_ArrayIsPopulated(ImmutableArray<string> immutableArray)
    {
        immutableArray.Should().NotBeEmpty();
        immutableArray.Should().OnlyContain(x => !string.IsNullOrEmpty(x));
    }

    [Test]
    [AutoData]
    public void AutoData_GeneratesImmutableList_ListIsPopulated(ImmutableList<int> immutableList)
    {
        immutableList.Should().NotBeEmpty();
        immutableList.Should().OnlyContain(x => x != 0);
    }

    [Test]
    [AutoData]
    public void AutoData_GeneratesImmutableDictionary_DictionaryIsPopulated(
        ImmutableDictionary<string, int> immutableDictionary)
    {
        immutableDictionary.Should().NotBeEmpty();
        immutableDictionary.Keys.Should().OnlyContain(k => !string.IsNullOrEmpty(k));
        immutableDictionary.Values.Should().OnlyContain(v => v != 0);
    }

    [Test]
    [AutoData]
    public void AutoData_GeneratesImmutableHashSet_HashSetIsPopulated(ImmutableHashSet<string> immutableHashSet)
    {
        immutableHashSet.Should().NotBeEmpty();
        immutableHashSet.Should().OnlyContain(x => !string.IsNullOrEmpty(x));
    }

    [Test]
    [AutoData]
    public void AutoData_GeneratesImmutableSortedSet_SortedSetIsPopulated(ImmutableSortedSet<int> immutableSortedSet)
    {
        immutableSortedSet.Should().NotBeEmpty();
        immutableSortedSet.Should().OnlyContain(x => x != 0);
    }

    [Test]
    [AutoData]
    public void AutoData_GeneratesImmutableSortedDictionary_SortedDictionaryIsPopulated(
        ImmutableSortedDictionary<string, int> immutableSortedDictionary)
    {
        immutableSortedDictionary.Should().NotBeEmpty();
        immutableSortedDictionary.Keys.Should().OnlyContain(k => !string.IsNullOrEmpty(k));
        immutableSortedDictionary.Values.Should().OnlyContain(v => v != 0);
    }

    [Test]
    [AutoData]
    public void AutoData_GeneratesMultipleImmutableTypes_AllArePopulated(
        ImmutableArray<string> array,
        ImmutableList<int> list,
        ImmutableDictionary<string, int> dictionary)
    {
        array.Should().NotBeEmpty();
        list.Should().NotBeEmpty();
        dictionary.Should().NotBeEmpty();
    }

    [Test]
    [AutoData]
    public void AutoData_GeneratesComplexImmutableTypes_TypesArePopulated(
        ImmutableArray<Person> people,
        ImmutableList<Address> addresses)
    {
        people.Should().NotBeEmpty();
        people.Should().OnlyContain(p => !string.IsNullOrEmpty(p.Name));

        addresses.Should().NotBeEmpty();
        addresses.Should().OnlyContain(a => !string.IsNullOrEmpty(a.Street));
    }

    [Test]
    [InlineAutoData(5)]
    public void InlineAutoData_GeneratesImmutableCollections_WithExplicitAndAutoValues(
        int explicitValue,
        ImmutableList<string> autoList)
    {
        explicitValue.Should().Be(5);
        autoList.Should().NotBeEmpty();
        autoList.Should().OnlyContain(x => !string.IsNullOrEmpty(x));
    }

    public class Person
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    public class Address
    {
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }
}
#endif
