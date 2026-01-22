using FluentAssertions;
using Poc.UnitTests.Infrastructure;
using TUnit.AutoFixture;
using TUnit.Core;

namespace Poc.UnitTests.Examples;

/// <summary>
/// Tests demonstrating frozen dependency injection with various matching strategies.
/// </summary>
public class FrozenDependencyTests
{
    [Test]
    [AutoNSubstituteData]
    public void Frozen_ExactType_SameInstanceInjected(
        [Frozen] IService service,
        Consumer consumer)
    {
        // Assert - the frozen IService instance should be injected into the Consumer
        consumer.Should().NotBeNull();
        consumer.Service.Should().BeSameAs(service);
    }

    [Test]
    [AutoNSubstituteData]
    public void Frozen_MultipleConsumers_SameServiceInstance(
        [Frozen] IService service,
        Consumer consumer1,
        Consumer consumer2)
    {
        // Assert - all consumers should receive the same frozen service instance
        consumer1.Service.Should().BeSameAs(service);
        consumer2.Service.Should().BeSameAs(service);
        consumer1.Service.Should().BeSameAs(consumer2.Service);
    }

    [Test]
    [AutoNSubstituteData]
    public void Frozen_ExactType_ConcreteTypeMatching(
        [Frozen(Matching.ExactType)] ConcreteService concreteService,
        Consumer consumer)
    {
        // When frozen with ExactType, only exact type matches
        // Consumer depends on IService, so it should NOT get the frozen ConcreteService
        // (it will get a different IService instance)
        concreteService.Should().NotBeNull();
        consumer.Should().NotBeNull();
        consumer.Service.Should().NotBeNull();

        // They should be different instances since ConcreteService is frozen only for its exact type
        consumer.Service.Should().NotBeSameAs(concreteService);
    }

    [Test]
    [AutoNSubstituteData]
    public void Frozen_ImplementedInterfaces_InterfaceMatching(
        [Frozen(Matching.ImplementedInterfaces)] ConcreteService concreteService,
        Consumer consumer)
    {
        // When frozen with ImplementedInterfaces, interfaces are matched
        // Consumer depends on IService which ConcreteService implements
        concreteService.Should().NotBeNull();
        consumer.Should().NotBeNull();
        consumer.Service.Should().BeSameAs(concreteService);
    }

    [Test]
    [AutoNSubstituteData]
    public void Frozen_DirectBaseType_BaseClassMatching(
        [Frozen(Matching.DirectBaseType)] DerivedClass derived,
        BaseClass baseInstance)
    {
        // When frozen with DirectBaseType, the direct base class is matched
        derived.Should().NotBeNull();
        baseInstance.Should().NotBeNull();
        baseInstance.Should().BeSameAs(derived);
    }

    [Test]
    [AutoNSubstituteData]
    public void Frozen_MultipleInterfaces_AllInterfacesGetSameInstance(
        [Frozen(Matching.ImplementedInterfaces)] MultiInterfaceClass implementation,
        IInterface1 interface1,
        IInterface2 interface2)
    {
        // When frozen with ImplementedInterfaces, all interfaces should get the same instance
        implementation.Should().NotBeNull();
        interface1.Should().BeSameAs(implementation);
        interface2.Should().BeSameAs(implementation);
    }

    [Test]
    [AutoNSubstituteData]
    public void Frozen_MemberOfFamily_AllRelatedTypesMatch(
        [Frozen(Matching.MemberOfFamily)] DerivedClass derived,
        BaseClass baseInstance,
        DerivedClass anotherDerived)
    {
        // MemberOfFamily should match both base types and the exact type
        derived.Should().NotBeNull();
        baseInstance.Should().BeSameAs(derived);
        anotherDerived.Should().BeSameAs(derived);
    }

    [Test]
    [AutoNSubstituteData]
    public void Frozen_ComplexDependencyGraph_ConsistentInjection(
        [Frozen] IService service,
        [Frozen] Person person,
        [Frozen] Address address,
        MultiDependencyConsumer consumer)
    {
        // Assert - all frozen dependencies should be injected consistently
        consumer.Should().NotBeNull();
        consumer.Service.Should().BeSameAs(service);
        consumer.Person.Should().BeSameAs(person);
        consumer.Address.Should().BeSameAs(address);
    }

    [Test]
    [AutoData]
    public void Frozen_AppliesGloballyToType_AllInstancesAreSame(
        [Frozen] Person frozenPerson,
        Person anotherPerson)
    {
        // IMPORTANT: When a parameter is marked with [Frozen], AutoFixture freezes
        // that type for the ENTIRE fixture instance. This means ALL subsequent
        // requests for that type will return the same instance.
        // This is standard AutoFixture behavior, not a limitation of this library.
        frozenPerson.Should().NotBeNull();
        anotherPerson.Should().NotBeNull();

        // Both Person parameters receive the same frozen instance
        // This is expected AutoFixture behavior
        anotherPerson.Should().BeSameAs(frozenPerson);
    }
}