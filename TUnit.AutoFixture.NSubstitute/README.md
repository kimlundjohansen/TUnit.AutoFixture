# TUnit.AutoFixture.NSubstitute

NSubstitute auto-mocking integration for TUnit.AutoFixture.

## Installation

```bash
dotnet add package TUnit.AutoFixture.NSubstitute
```

This package automatically includes:
- TUnit.AutoFixture (core library)
- AutoFixture.AutoNSubstitute
- NSubstitute

## Features

- **`[AutoNSubstituteData]`** - Automatically mocks interfaces and generates test data
- **`[InlineAutoNSubstituteData]`** - Combines explicit values with auto-mocked interfaces
- **Frozen Mocks** - Use `[Frozen]` with mocked interfaces
- **Full NSubstitute API** - Configure mocks with `Returns()`, `Received()`, etc.
- **xUnit-Compatible** - Same patterns as AutoFixture.AutoNSubstitute

## Usage

### Basic Auto-Mocking with [AutoNSubstituteData]

The `[AutoNSubstituteData]` attribute automatically mocks all interfaces:

```csharp
using TUnit.AutoFixture.NSubstitute;
using TUnit.Core;
using NSubstitute;

public interface IRepository
{
    string GetData();
}

[Test]
[AutoNSubstituteData]
public void Test_AutomaticallyMocksInterfaces(IRepository repository)
{
    // Interface is automatically mocked by NSubstitute
    repository.Should().NotBeNull();

    // Configure the mock
    repository.GetData().Returns("test data");

    // Use the mock
    var result = repository.GetData();
    result.Should().Be("test data");
}
```

### Combining Mocks and Generated Types

```csharp
public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
}

[Test]
[AutoNSubstituteData]
public void Test_MocksAndGeneratedTypes(
    IRepository mockRepository,
    string autoText,
    int autoNumber,
    Person autoPerson)
{
    // Interfaces are mocked
    mockRepository.Should().NotBeNull();

    // Primitives are auto-generated
    autoText.Should().NotBeNull();
    autoNumber.Should().NotBe(0);

    // Complex types are auto-generated
    autoPerson.Should().NotBeNull();
    autoPerson.Name.Should().NotBeNull();
}
```

## Frozen Mocks

Use `[Frozen]` to inject the same mock instance into all consumers:

```csharp
public interface IService
{
    string GetData();
}

public class Service
{
    private readonly IService _service;

    public Service(IService service)
    {
        _service = service;
    }

    public string ProcessData()
    {
        return _service.GetData();
    }
}

[Test]
[AutoNSubstituteData]
public void Test_FrozenMock(
    [Frozen] IService service,
    Service sut)
{
    // Configure the frozen mock
    service.GetData().Returns("frozen data");

    // Act - sut uses the same frozen mock
    var result = sut.ProcessData();

    // Assert
    result.Should().Be("frozen data");
    service.Received(1).GetData();
}
```

### Multiple Consumers, Same Mock

```csharp
public class Consumer
{
    public IService Service { get; }

    public Consumer(IService service)
    {
        Service = service;
    }
}

[Test]
[AutoNSubstituteData]
public void Test_MultipleFrozenMockInjections(
    [Frozen] IService service,
    Consumer consumer1,
    Consumer consumer2)
{
    // All consumers receive the same frozen mock
    consumer1.Service.Should().BeSameAs(service);
    consumer2.Service.Should().BeSameAs(service);
}
```

## Inline Auto-Mocking with [InlineAutoNSubstituteData]

Combine explicit values with auto-mocked interfaces:

```csharp
[Test]
[InlineAutoNSubstituteData("explicit value")]
public void Test_InlineWithMocks(
    string explicitValue,
    IRepository autoMock,
    Person autoPerson)
{
    // First parameter is explicit
    explicitValue.Should().Be("explicit value");

    // Interfaces are auto-mocked
    autoMock.Should().NotBeNull();
    autoMock.GetData().Returns("mocked");
    autoMock.GetData().Should().Be("mocked");

    // Complex types are auto-generated
    autoPerson.Should().NotBeNull();
}
```

### Multiple Test Cases with Mocks

```csharp
[Test]
[InlineAutoNSubstituteData(100)]
[InlineAutoNSubstituteData(200)]
public void Test_MultipleTestCasesWithMocks(
    int explicitId,
    IRepository repository)
{
    // Each test case has its own mock instance
    repository.GetData().Returns($"data-{explicitId}");

    var result = repository.GetData();

    result.Should().Be($"data-{explicitId}");
    explicitId.Should().BeOneOf(100, 200);
}
```

## NSubstitute Features

### Configuring Return Values

```csharp
[Test]
[AutoNSubstituteData]
public void Test_ConfiguringReturns(IService service)
{
    // Simple return value
    service.GetData().Returns("test");

    // Conditional returns
    service.GetData("key1").Returns("value1");
    service.GetData("key2").Returns("value2");

    // Computed returns
    service.GetData(Arg.Any<string>()).Returns(x => $"Result for {x[0]}");
}
```

### Verifying Method Calls

```csharp
[Test]
[AutoNSubstituteData]
public void Test_VerifyingCalls(IService service)
{
    // Arrange
    service.GetData().Returns("test");

    // Act - call multiple times
    service.GetData();
    service.GetData();
    service.GetData();

    // Assert - verify call count
    service.Received(3).GetData();

    // Verify with argument matchers
    service.GetData("specific");
    service.Received(1).GetData("specific");
}
```

### Argument Matchers

```csharp
[Test]
[AutoNSubstituteData]
public void Test_ArgumentMatchers(IService service)
{
    // Any argument
    service.GetData(Arg.Any<string>()).Returns("default");

    // Specific conditions
    service.GetData(Arg.Is<string>(x => x.StartsWith("test"))).Returns("matched");

    // Multiple conditions
    service.GetData(
        Arg.Is<string>(x => x.Length > 5),
        Arg.Any<int>()
    ).Returns("complex match");
}
```

### Throwing Exceptions

```csharp
[Test]
[AutoNSubstituteData]
public void Test_ThrowingExceptions(IService service)
{
    // Configure mock to throw
    service.GetData()
        .Returns(x => throw new InvalidOperationException("Test exception"));

    // Act & Assert
    Action act = () => service.GetData();
    act.Should().Throw<InvalidOperationException>()
       .WithMessage("Test exception");
}
```

## Advanced Scenarios

### Multiple Interface Implementations

```csharp
public interface IInterface1
{
    void Method1();
}

public interface IInterface2
{
    void Method2();
}

public class MultiInterfaceClass : IInterface1, IInterface2
{
    public void Method1() { }
    public void Method2() { }
}

[Test]
[AutoNSubstituteData]
public void Test_MultipleInterfaces(
    IInterface1 interface1,
    IInterface2 interface2,
    MultiInterfaceClass concrete)
{
    // All interfaces are mocked
    interface1.Should().NotBeNull();
    interface2.Should().NotBeNull();

    // Concrete class is generated
    concrete.Should().NotBeNull();

    // Configure and verify mocks
    interface1.When(x => x.Method1()).Do(_ => { });
    interface2.When(x => x.Method2()).Do(_ => { });

    interface1.Method1();
    interface2.Method2();

    interface1.Received(1).Method1();
    interface2.Received(1).Method2();
}
```

### Frozen with Matching Strategies

Combine frozen mocks with matching strategies:

```csharp
public interface IService { }
public class ConcreteService : IService { }

public class Consumer
{
    public IService Service { get; }

    public Consumer(IService service)
    {
        Service = service;
    }
}

[Test]
[AutoNSubstituteData]
public void Test_FrozenWithImplementedInterfaces(
    [Frozen(Matching.ImplementedInterfaces)] ConcreteService concrete,
    Consumer consumer)
{
    // With ImplementedInterfaces, concrete is frozen for IService too
    consumer.Service.Should().BeSameAs(concrete);
}
```

### Complex Dependency Graphs

```csharp
public class MultiDependencyService
{
    public IRepository Repository { get; }
    public ILogger Logger { get; }
    public ICache Cache { get; }

    public MultiDependencyService(
        IRepository repository,
        ILogger logger,
        ICache cache)
    {
        Repository = repository;
        Logger = logger;
        Cache = cache;
    }
}

[Test]
[AutoNSubstituteData]
public void Test_ComplexDependencyGraph(
    [Frozen] IRepository repository,
    [Frozen] ILogger logger,
    [Frozen] ICache cache,
    MultiDependencyService sut)
{
    // All frozen mocks are injected
    sut.Repository.Should().BeSameAs(repository);
    sut.Logger.Should().BeSameAs(logger);
    sut.Cache.Should().BeSameAs(cache);

    // Configure and verify
    repository.GetData().Returns("data");
    cache.TryGet(Arg.Any<string>(), out Arg.Any<object>()).Returns(false);

    // Use sut...
}
```

## ConfigureMembers Setting

This library uses `ConfigureMembers = false` by default, which means NSubstitute won't automatically configure property getters and setters.

If you need automatic property configuration:

```csharp
public class CustomAutoNSubstituteDataAttribute : AutoDataAttribute
{
    public CustomAutoNSubstituteDataAttribute()
        : base(() =>
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization
            {
                ConfigureMembers = true  // Enable automatic property configuration
            });
            return fixture;
        })
    {
    }
}

[Test]
[CustomAutoNSubstituteData]
public void Test_WithConfiguredMembers(IService service)
{
    // Properties are automatically configured
}
```

## Important Notes

### Multiple Mock Instances

Each parameter gets its own mock instance unless marked with `[Frozen]`:

```csharp
[Test]
[AutoNSubstituteData]
public void Test_MultipleMocks(
    IService service1,
    IService service2)
{
    // These are DIFFERENT mock instances
    service1.Should().NotBeSameAs(service2);

    // Each can be configured independently
    service1.GetData().Returns("mock1");
    service2.GetData().Returns("mock2");

    service1.GetData().Should().Be("mock1");
    service2.GetData().Should().Be("mock2");
}
```

### Concrete Types Not Mocked

Only interfaces and abstract classes are mocked. Concrete types are generated by AutoFixture:

```csharp
public class ConcreteClass
{
    public virtual string GetData() => "real";
}

[Test]
[AutoNSubstituteData]
public void Test_ConcreteTypesNotMocked(ConcreteClass concrete)
{
    // This is NOT a mock - it's a real instance
    concrete.Should().NotBeNull();
    concrete.GetData().Should().Be("real");
}
```

To mock concrete classes with virtual members, use `Substitute.ForPartsOf<T>()` manually.

## Examples

See the [test project](../TUnit.AutoFixture.UnitTests/Examples/) for comprehensive examples:

- **[AutoNSubstituteTests.cs](../TUnit.AutoFixture.UnitTests/Examples/AutoNSubstituteTests.cs)** - All auto-mocking scenarios

## Requirements

- .NET 10.0 or later
- TUnit.Core 1.10.0 or later
- TUnit.AutoFixture 1.0.0 or later
- AutoFixture.AutoNSubstitute 4.18.1 or later
- NSubstitute 5.1.0 or later

## NSubstitute Documentation

For complete NSubstitute documentation, see:
- [NSubstitute Official Documentation](https://nsubstitute.github.io/)
- [NSubstitute GitHub](https://github.com/nsubstitute/NSubstitute)

## License

MIT License - see [LICENSE](../LICENSE) file for details.
