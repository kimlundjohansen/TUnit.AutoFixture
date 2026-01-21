# TUnit.AutoFixture

Core library for auto-generating test data in TUnit using AutoFixture.

## Installation

```bash
dotnet add package TUnit.AutoFixture
```

## Features

- **`[AutoData]`** - Automatically generates all test parameters
- **`[InlineAutoData]`** - Combines explicit values with auto-generated parameters
- **`[Frozen]`** - Freezes a parameter to be reused throughout the object graph
- **5 Matching Strategies** - Control how frozen parameters match types
- **xUnit-Compatible** - Drop-in replacement for xUnit.AutoFixture attribute names

## Usage

### Basic Auto-Generation with [AutoData]

The `[AutoData]` attribute automatically generates all test parameters using AutoFixture:

```csharp
using TUnit.AutoFixture;
using TUnit.Core;

[Test]
[AutoData]
public void Test_AutomaticallyGeneratesAllParameters(
    string text,
    int number,
    bool flag,
    Guid guid)
{
    // All parameters are automatically populated with non-default values
    text.Should().NotBeNull();
    number.Should().NotBe(0);
    guid.Should().NotBe(Guid.Empty);
}
```

### Complex Types

AutoFixture automatically creates complex types with populated properties:

```csharp
public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
    public Address Address { get; set; }
}

public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
}

[Test]
[AutoData]
public void Test_ComplexTypesArePopulated(Person person)
{
    person.Should().NotBeNull();
    person.Name.Should().NotBeNull();
    person.Age.Should().NotBe(0);
    person.Address.Should().NotBeNull();
    person.Address.Street.Should().NotBeNull();
}
```

### Collections and Enums

Collections are automatically populated with multiple items:

```csharp
[Test]
[AutoData]
public void Test_CollectionsAndEnums(
    List<int> numbers,
    DayOfWeek dayOfWeek)
{
    numbers.Should().NotBeNull();
    numbers.Should().NotBeEmpty();
    numbers.Count.Should().BeGreaterThan(0);

    Enum.IsDefined(typeof(DayOfWeek), dayOfWeek).Should().BeTrue();
}
```

## Inline Data with [InlineAutoData]

The `[InlineAutoData]` attribute combines explicit values with auto-generated parameters:

```csharp
[Test]
[InlineAutoData("explicit string", 42)]
public void Test_CombinesExplicitAndAuto(
    string explicitText,
    int explicitNumber,
    Person autoPerson,
    Address autoAddress)
{
    // First two parameters use explicit values
    explicitText.Should().Be("explicit string");
    explicitNumber.Should().Be(42);

    // Remaining parameters are auto-generated
    autoPerson.Should().NotBeNull();
    autoAddress.Should().NotBeNull();
}
```

### Multiple Test Cases

Use multiple `[InlineAutoData]` attributes to create multiple test cases:

```csharp
[Test]
[InlineAutoData("John", 25)]
[InlineAutoData("Jane", 30)]
[InlineAutoData("Bob", 35)]
public void Test_MultipleTestCases(
    string name,
    int age,
    Address address)
{
    // This test runs 3 times with different explicit values
    name.Should().BeOneOf("John", "Jane", "Bob");
    age.Should().BeOneOf(25, 30, 35);

    // Auto-generated parameter is different for each test case
    address.Should().NotBeNull();
}
```

### All Explicit Values

If you provide values for all parameters, no auto-generation occurs:

```csharp
[Test]
[InlineAutoData(100, 200, 300)]
public void Test_AllExplicitValues(int value1, int value2, int value3)
{
    value1.Should().Be(100);
    value2.Should().Be(200);
    value3.Should().Be(300);
}
```

### No Explicit Values

`[InlineAutoData()]` with no arguments behaves identically to `[AutoData]`:

```csharp
[Test]
[InlineAutoData()]
public void Test_NoExplicitValues(string text, int number)
{
    // Behaves exactly like [AutoData]
    text.Should().NotBeNull();
    number.Should().NotBe(0);
}
```

## Frozen Parameters with [Frozen]

The `[Frozen]` attribute marks a parameter to be "frozen" - reused throughout the object graph:

```csharp
public interface IService { }
public class Consumer
{
    public IService Service { get; }

    public Consumer(IService service)
    {
        Service = service;
    }
}

[Test]
[AutoData]
public void Test_FrozenParameter(
    [Frozen] IService service,
    Consumer consumer)
{
    // The same IService instance is injected into Consumer
    consumer.Service.Should().BeSameAs(service);
}
```

### Multiple Consumers, Same Frozen Instance

```csharp
[Test]
[AutoData]
public void Test_MultipleFrozenInjections(
    [Frozen] IService service,
    Consumer consumer1,
    Consumer consumer2)
{
    // All consumers receive the same frozen instance
    consumer1.Service.Should().BeSameAs(service);
    consumer2.Service.Should().BeSameAs(service);
    consumer1.Service.Should().BeSameAs(consumer2.Service);
}
```

## Matching Strategies

The `[Frozen]` attribute supports 5 matching strategies that control how the frozen instance is matched:

### 1. ExactType (Default)

Matches only the exact type:

```csharp
public class ConcreteService : IService { }

[Test]
[AutoData]
public void Test_ExactTypeMatching(
    [Frozen(Matching.ExactType)] ConcreteService concrete,
    Consumer consumer)
{
    // Consumer depends on IService, not ConcreteService
    // So it gets a DIFFERENT instance
    consumer.Service.Should().NotBeSameAs(concrete);
}
```

### 2. ImplementedInterfaces

Freezes the instance for all interfaces it implements:

```csharp
[Test]
[AutoData]
public void Test_ImplementedInterfaces(
    [Frozen(Matching.ImplementedInterfaces)] ConcreteService concrete,
    Consumer consumer)
{
    // ConcreteService implements IService
    // So Consumer receives the frozen concrete instance
    consumer.Service.Should().BeSameAs(concrete);
}
```

### 3. DirectBaseType

Freezes for the direct base class only:

```csharp
public class BaseClass { }
public class DerivedClass : BaseClass { }

[Test]
[AutoData]
public void Test_DirectBaseType(
    [Frozen(Matching.DirectBaseType)] DerivedClass derived,
    BaseClass baseInstance)
{
    // DerivedClass is frozen for its direct base (BaseClass)
    baseInstance.Should().BeSameAs(derived);
}
```

### 4. BaseType

Freezes for all base classes in the inheritance chain:

```csharp
public class GrandparentClass { }
public class ParentClass : GrandparentClass { }
public class ChildClass : ParentClass { }

[Test]
[AutoData]
public void Test_BaseType(
    [Frozen(Matching.BaseType)] ChildClass child,
    ParentClass parent,
    GrandparentClass grandparent)
{
    // Frozen for all base classes
    parent.Should().BeSameAs(child);
    grandparent.Should().BeSameAs(child);
}
```

### 5. MemberOfFamily

Combines interfaces, base classes, and exact type:

```csharp
[Test]
[AutoData]
public void Test_MemberOfFamily(
    [Frozen(Matching.MemberOfFamily)] DerivedClass derived,
    BaseClass baseInstance,
    IService service,
    DerivedClass anotherDerived)
{
    // Matches everything: base classes, interfaces, and exact type
    baseInstance.Should().BeSameAs(derived);
    service.Should().BeSameAs(derived);
    anotherDerived.Should().BeSameAs(derived);
}
```

## Matching Strategy Comparison

| Strategy | Exact Type | Base Classes | Interfaces |
|----------|-----------|--------------|------------|
| **ExactType** | ✅ | ❌ | ❌ |
| **ImplementedInterfaces** | ✅ | ❌ | ✅ |
| **DirectBaseType** | ✅ | ✅ (direct only) | ❌ |
| **BaseType** | ✅ | ✅ (all) | ❌ |
| **MemberOfFamily** | ✅ | ✅ (all) | ✅ |

## Complex Dependency Graphs

Multiple frozen parameters can be combined:

```csharp
public class MultiDependencyConsumer
{
    public IService Service { get; }
    public Person Person { get; }
    public Address Address { get; }

    public MultiDependencyConsumer(
        IService service,
        Person person,
        Address address)
    {
        Service = service;
        Person = person;
        Address = address;
    }
}

[Test]
[AutoData]
public void Test_ComplexDependencyGraph(
    [Frozen] IService service,
    [Frozen] Person person,
    [Frozen] Address address,
    MultiDependencyConsumer consumer)
{
    // All frozen instances are injected
    consumer.Service.Should().BeSameAs(service);
    consumer.Person.Should().BeSameAs(person);
    consumer.Address.Should().BeSameAs(address);
}
```

## Important AutoFixture Behavior

### Global Type Freezing

When a parameter is marked with `[Frozen]`, AutoFixture freezes that type for the **entire fixture instance**:

```csharp
[Test]
[AutoData]
public void Test_FreezeAppliesGlobally(
    [Frozen] Person frozenPerson,
    Person anotherPerson)
{
    // Both Person parameters receive the SAME frozen instance
    // This is standard AutoFixture behavior
    anotherPerson.Should().BeSameAs(frozenPerson);
}
```

This is **expected AutoFixture behavior**, not a limitation of this library.

## Customization

### Custom Fixture Creation

You can create derived attributes with custom fixture configuration:

```csharp
public class CustomAutoDataAttribute : AutoDataAttribute
{
    public CustomAutoDataAttribute()
        : base(() =>
        {
            var fixture = new Fixture();

            // Add your customizations
            fixture.Customize(new MyCustomCustomization());

            return fixture;
        })
    {
    }
}

[Test]
[CustomAutoData]
public void Test_WithCustomFixture(Person person)
{
    // Uses your custom fixture configuration
}
```

## Thread Safety

Each test method invocation creates a new fixture instance using `Lazy<IFixture>` with `PublicationOnly` thread safety mode. This ensures tests can run in parallel without state conflicts.

## Performance Considerations

AutoFixture uses reflection to create instances. For performance-critical scenarios:

1. Use `[InlineAutoData]` to provide explicit values for complex types
2. Create custom specimen builders for frequently-used types
3. Consider caching fixture instances in custom attributes

## Limitations

### TUnit Null Literal Handling

TUnit's attribute mechanism does not preserve `null` literal values passed to attributes:

```csharp
[Test]
[InlineAutoData(null!)]  // Will NOT preserve null
public void Test_NullValue(string? nullable)
{
    // nullable will NOT be null - it gets replaced with a generated string
    // This is a TUnit limitation, not an AutoFixture or this library limitation
}
```

**Workaround**: Use nullable reference types and test null scenarios separately without `[InlineAutoData]`.

## API Reference

### Attributes

#### `[AutoData]`
Automatically generates all test method parameters using AutoFixture.

```csharp
public class AutoDataAttribute : UntypedDataSourceGeneratorAttribute
```

#### `[InlineAutoData(params object?[] values)]`
Combines explicit values with auto-generated parameters.

```csharp
public class InlineAutoDataAttribute : AutoDataAttribute
```

**Parameters:**
- `values` - Explicit values for the first N parameters

#### `[Frozen(Matching matching = Matching.ExactType)]`
Marks a parameter to be frozen and reused in the object graph.

```csharp
public class FrozenAttribute : CustomizeAttribute
```

**Parameters:**
- `matching` - The matching strategy (default: `ExactType`)

### Enums

#### `Matching`
Defines how frozen parameters match types:

```csharp
public enum Matching
{
    ExactType,
    ImplementedInterfaces,
    DirectBaseType,
    BaseType,
    MemberOfFamily
}
```

## Examples

See the [test project](../TUnit.AutoFixture.UnitTests/Examples/) for comprehensive examples:

- **[BasicAutoDataTests.cs](../TUnit.AutoFixture.UnitTests/Examples/BasicAutoDataTests.cs)** - Basic auto-generation patterns
- **[FrozenDependencyTests.cs](../TUnit.AutoFixture.UnitTests/Examples/FrozenDependencyTests.cs)** - All matching strategies
- **[InlineAutoDataTests.cs](../TUnit.AutoFixture.UnitTests/Examples/InlineAutoDataTests.cs)** - Inline data combinations

## Requirements

- .NET 9.0 or later
- TUnit.Core 1.10.0 or later
- AutoFixture 4.18.1 or later

## License

MIT License - see [LICENSE](../LICENSE) file for details.
