namespace Poc.UnitTests.Infrastructure;

/// <summary>
/// Test subject representing a person.
/// </summary>
public class Person
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public Address? Address { get; set; }
}

/// <summary>
/// Test subject representing an address.
/// </summary>
public class Address
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
}

/// <summary>
/// Test interface for dependency injection scenarios.
/// </summary>
public interface IService
{
    string GetData();
}

/// <summary>
/// Test implementation of IService.
/// </summary>
public class ConcreteService : IService
{
    public string GetData() => "ConcreteService";
}

/// <summary>
/// Test consumer that depends on IService.
/// </summary>
public class Consumer
{
    public Consumer(IService service)
    {
        Service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public IService Service { get; }

    public string GetServiceData() => Service.GetData();
}

/// <summary>
/// Test class with multiple dependencies.
/// </summary>
public class MultiDependencyConsumer
{
    public MultiDependencyConsumer(IService service, Person person, Address address)
    {
        Service = service ?? throw new ArgumentNullException(nameof(service));
        Person = person ?? throw new ArgumentNullException(nameof(person));
        Address = address ?? throw new ArgumentNullException(nameof(address));
    }

    public IService Service { get; }
    public Person Person { get; }
    public Address Address { get; }
}

/// <summary>
/// Base class for testing inheritance matching.
/// </summary>
public class BaseClass
{
    public string BaseProperty { get; set; } = string.Empty;
}

/// <summary>
/// Derived class for testing inheritance matching.
/// </summary>
public class DerivedClass : BaseClass
{
    public string DerivedProperty { get; set; } = string.Empty;
}

/// <summary>
/// Interface for testing interface matching.
/// </summary>
public interface IInterface1
{
    void Method1();
}

/// <summary>
/// Another interface for testing interface matching.
/// </summary>
public interface IInterface2
{
    void Method2();
}

/// <summary>
/// Class implementing multiple interfaces for testing interface matching.
/// </summary>
public class MultiInterfaceClass : IInterface1, IInterface2
{
    public void Method1()
    {
    }

    public void Method2()
    {
    }
}