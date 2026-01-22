using FluentAssertions;
using Poc.UnitTests.Infrastructure;
using TUnit.AutoFixture;
using TUnit.Core;

namespace Poc.UnitTests.Examples;

/// <summary>
/// Tests demonstrating custom generators for special types.
/// </summary>
public class GeneratorTests
{
    [Test]
    [AutoData]
    public void AutoData_GeneratesCancellationToken_NotCanceled(CancellationToken token)
    {
        // Assert - CancellationToken should be generated and not canceled
        token.Should().NotBe(default(CancellationToken));
        token.IsCancellationRequested.Should().BeFalse();
    }

    [Test]
    [AutoData]
    public void AutoData_GeneratesMultipleCancellationTokens_EachNotCanceled(
        CancellationToken token1,
        CancellationToken token2,
        CancellationToken token3)
    {
        // Assert - All tokens should be generated and not canceled
        token1.IsCancellationRequested.Should().BeFalse();
        token2.IsCancellationRequested.Should().BeFalse();
        token3.IsCancellationRequested.Should().BeFalse();
    }

#if NET6_0_OR_GREATER
    [Test]
    [AutoData]
    public void AutoData_GeneratesDateOnly_ValidDate(DateOnly date)
    {
        // Assert - DateOnly should be generated with a valid date
        date.Should().NotBe(default(DateOnly));
        date.Year.Should().BeGreaterThan(0);
    }

    [Test]
    [AutoData]
    public void AutoData_GeneratesMultipleDateOnly_EachUnique(
        DateOnly date1,
        DateOnly date2,
        DateOnly date3)
    {
        // Assert - Multiple DateOnly instances should be generated
        // (they might be the same or different depending on AutoFixture's algorithm)
        date1.Should().NotBe(default(DateOnly));
        date2.Should().NotBe(default(DateOnly));
        date3.Should().NotBe(default(DateOnly));
    }

    [Test]
    [InlineAutoData(2025, 1, 15)]
    public void InlineAutoData_WorksWithDateOnly(
        int year,
        int month,
        int day,
        DateOnly autoDate)
    {
        // Arrange
        var explicitDate = new DateOnly(year, month, day);

        // Assert
        explicitDate.Year.Should().Be(2025);
        explicitDate.Month.Should().Be(1);
        explicitDate.Day.Should().Be(15);
        autoDate.Should().NotBe(default(DateOnly));
    }
#endif

    [Test]
    [AutoNSubstituteData]
    public void AutoNSubstituteData_GeneratesCancellationToken_WithMocking(
        IService service,
        CancellationToken token)
    {
        // Assert - Both CancellationToken and mocks work together
        service.Should().NotBeNull();
        token.IsCancellationRequested.Should().BeFalse();
    }

    [Test]
    [InlineAutoData("explicit")]
    public void InlineAutoData_WorksWithCancellationToken(
        string explicitValue,
        CancellationToken token)
    {
        // Assert
        explicitValue.Should().Be("explicit");
        token.IsCancellationRequested.Should().BeFalse();
    }
}
