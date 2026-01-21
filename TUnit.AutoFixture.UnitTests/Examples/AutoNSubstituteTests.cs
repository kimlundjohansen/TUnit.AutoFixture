using FluentAssertions;
using NSubstitute;
using Poc.UnitTests.Infrastructure;
using TUnit.AutoFixture;
using TUnit.AutoFixture.NSubstitute;

namespace Poc.UnitTests.Examples;

/// <summary>
/// Tests demonstrating AutoNSubstitute functionality with auto-mocked interfaces.
/// </summary>
public class AutoNSubstituteTests
{
    [Test]
    [AutoNSubstituteData]
    public void AutoNSubstitute_AutomaticallyMocksInterfaces_MockIsCreated(IService service)
    {
        // Assert - interface is automatically mocked
        service.Should().NotBeNull();

        // Can configure the mock
        service.GetData().Returns("test data");

        // Assert - mock works correctly
        service.GetData().Should().Be("test data");
    }

    [Test]
    [AutoNSubstituteData]
    public void AutoNSubstitute_CombinesMocksAndGeneratedTypes_BothAreCreated(
        IService mockService,
        string autoText,
        int autoNumber,
        Person autoPerson)
    {
        // Assert - mock is created
        mockService.Should().NotBeNull();

        // Assert - primitive types are generated
        autoText.Should().NotBeNull();
        autoNumber.Should().NotBe(0);

        // Assert - complex types are generated
        autoPerson.Should().NotBeNull();
        autoPerson.Name.Should().NotBeNull();
    }

    [Test]
    [AutoNSubstituteData]
    public void AutoNSubstitute_FrozenMock_SameInstanceInjected(
        [Frozen] IService service,
        Consumer consumer)
    {
        // Arrange - configure the frozen mock
        service.GetData().Returns("frozen data");

        // Act - consumer should use the same frozen mock
        var result = consumer.GetServiceData();

        // Assert - frozen mock was injected
        result.Should().Be("frozen data");
        service.Received(1).GetData();
    }

    [Test]
    [AutoNSubstituteData]
    public void AutoNSubstitute_MultipleMocks_EachIsIndependent(
        IService service1,
        IService service2)
    {
        // Arrange - configure each mock differently
        service1.GetData().Returns("service1 data");
        service2.GetData().Returns("service2 data");

        // Assert - each mock behaves independently
        service1.GetData().Should().Be("service1 data");
        service2.GetData().Should().Be("service2 data");

        // Assert - they are different instances
        service1.Should().NotBeSameAs(service2);
    }

    [Test]
    [AutoNSubstituteData]
    public void AutoNSubstitute_ComplexDependencyGraph_AllMocksInjected(
        [Frozen] IService service,
        [Frozen] Person person,
        [Frozen] Address address,
        MultiDependencyConsumer consumer)
    {
        // Assert - all frozen dependencies are injected
        consumer.Should().NotBeNull();
        consumer.Service.Should().BeSameAs(service);
        consumer.Person.Should().BeSameAs(person);
        consumer.Address.Should().BeSameAs(address);
    }

    [Test]
    [AutoNSubstituteData]
    public void AutoNSubstitute_VerifyMockCalls_CallsAreRecorded(IService service)
    {
        // Arrange
        service.GetData().Returns("test");

        // Act - call the method multiple times
        service.GetData();
        service.GetData();
        service.GetData();

        // Assert - verify the calls were recorded
        service.Received(3).GetData();
    }

    [Test]
    [AutoNSubstituteData]
    public void AutoNSubstitute_WithArgMatching_MatchersWork(IService service)
    {
        // Arrange - setup for any string argument
        service.GetData().Returns("default");

        // Act
        var result = service.GetData();

        // Assert
        result.Should().Be("default");
        service.Received(1).GetData();
    }

    [Test]
    [InlineAutoNSubstituteData("explicit")]
    public void InlineAutoNSubstitute_CombinesExplicitAndMock_BothWork(
        string explicitValue,
        IService autoMock,
        Person autoPerson)
    {
        // Assert - explicit value
        explicitValue.Should().Be("explicit");

        // Assert - auto-mocked interface
        autoMock.Should().NotBeNull();
        autoMock.GetData().Returns("mocked");
        autoMock.GetData().Should().Be("mocked");

        // Assert - auto-generated type
        autoPerson.Should().NotBeNull();
    }

    [Test]
    [InlineAutoNSubstituteData(100)]
    [InlineAutoNSubstituteData(200)]
    public void InlineAutoNSubstitute_MultipleTestCases_EachHasOwnMock(
        int explicitId,
        IService service)
    {
        // Each test case has its own mock instance
        service.GetData().Returns($"data-{explicitId}");

        var result = service.GetData();

        result.Should().Be($"data-{explicitId}");
        explicitId.Should().BeOneOf(100, 200);
    }

    [Test]
    [InlineAutoNSubstituteData("test")]
    public void InlineAutoNSubstitute_WithFrozenMock_FrozenWorksCorrectly(
        string explicitValue,
        [Frozen] IService frozenService,
        Consumer consumer)
    {
        // Arrange
        explicitValue.Should().Be("test");
        frozenService.GetData().Returns("frozen");

        // Act
        var result = consumer.GetServiceData();

        // Assert - frozen mock was injected into consumer
        result.Should().Be("frozen");
        consumer.Service.Should().BeSameAs(frozenService);
    }

    [Test]
    [AutoNSubstituteData]
    public void AutoNSubstitute_WithMultipleInterfaces_AllAreMocked(
        IInterface1 interface1,
        IInterface2 interface2,
        MultiInterfaceClass concreteClass)
    {
        // Assert - interfaces are mocked
        interface1.Should().NotBeNull();
        interface2.Should().NotBeNull();

        // Assert - concrete class is generated
        concreteClass.Should().NotBeNull();

        // Verify mocks can be configured
        interface1.When(x => x.Method1()).Do(_ => { });
        interface2.When(x => x.Method2()).Do(_ => { });

        interface1.Method1();
        interface2.Method2();

        interface1.Received(1).Method1();
        interface2.Received(1).Method2();
    }

    [Test]
    [AutoNSubstituteData]
    public void AutoNSubstitute_MockBehaviorConfiguration_CustomBehaviorWorks(
        IService service)
    {
        // Arrange - configure mock to throw exception
        service.GetData().Returns(x => throw new InvalidOperationException("Test exception"));

        // Act & Assert
        try
        {
            service.GetData();
            false.Should().BeTrue(); // Should not reach here
        }
        catch (InvalidOperationException ex)
        {
            ex.Message.Should().Be("Test exception");
        }
    }

    [Test]
    [AutoNSubstituteData]
    public void AutoNSubstitute_FrozenWithMatching_InterfaceMatchingWorks(
        [Frozen(Matching.ImplementedInterfaces)] ConcreteService concrete,
        IService service)
    {
        // With ImplementedInterfaces matching, the concrete instance should be
        // frozen for its interface as well
        concrete.Should().NotBeNull();
        service.Should().BeSameAs(concrete);
    }
}
