# Release Notes

## Version 1.0.0 - Initial Release (2026-01-21)

First stable release of TUnit.AutoFixture - a streamlined, xUnit-compatible AutoFixture integration for TUnit.

### Features

#### Core Library (TUnit.AutoFixture)

- **`[AutoData]` Attribute** - Automatically generates all test method parameters using AutoFixture
  - Supports primitives, complex types, collections, enums, and nested objects
  - Thread-safe fixture creation with `Lazy<IFixture>` and `PublicationOnly` mode

- **`[InlineAutoData]` Attribute** - Combines explicit values with auto-generated parameters
  - Multiple `[InlineAutoData]` attributes create multiple test cases
  - Supports all explicit value types (primitives, arrays, enums, etc.)

- **`[Frozen]` Attribute** - Freezes parameters for dependency injection testing
  - 5 matching strategies: `ExactType`, `ImplementedInterfaces`, `DirectBaseType`, `BaseType`, `MemberOfFamily`
  - Supports complex dependency graphs with multiple frozen parameters
  - Works with both concrete types and interfaces

- **`Matching` Enum** - Controls how frozen parameters match types
  - Comprehensive XML documentation with examples for each strategy

- **xUnit Compatibility** - Drop-in replacement attribute naming
  - `[AutoData]` instead of `[AutoDataSource]`
  - `[InlineAutoData]` instead of `[AutoArguments]`
  - Identical API to AutoFixture.Xunit2

#### NSubstitute Integration (TUnit.AutoFixture.NSubstitute)

- **`[AutoNSubstituteData]` Attribute** - Automatically mocks interfaces using NSubstitute
  - Combines auto-mocking with auto-generated test data
  - Works with `[Frozen]` for consistent mock injection

- **`[InlineAutoNSubstituteData]` Attribute** - Combines explicit values, mocks, and generated data
  - Multiple attributes create multiple test cases with independent mocks

### Architecture

- **Simplified Design** - 6-8 core classes vs 20+ in official AutoFixture.TUnit
  - `AutoDataAttribute` - Base attribute extending `UntypedDataSourceGeneratorAttribute`
  - `InlineAutoDataAttribute` - Inherits from `AutoDataAttribute`
  - `FrozenAttribute` - Customization attribute for frozen parameters
  - `Matching` - Enum defining matching strategies
  - `AutoFixtureDataGenerator` - Internal generator with two-pass algorithm
  - `FreezeOnMatchCustomization` - Internal customization for frozen parameters

- **TUnit Integration** - Native integration with TUnit's source generator system
  - Uses `UntypedDataSourceGeneratorAttribute` as base class
  - Works with `IMemberMetadata` and `ParameterMetadata`
  - Compatible with TUnit 1.10.0 and later

### Quality

- **48 Passing Tests** - Comprehensive test coverage
  - 8 basic auto-data tests
  - 9 frozen dependency tests covering all matching strategies
  - 14 inline auto-data tests
  - 15 NSubstitute integration tests
  - 1 intentionally skipped test with documented TUnit limitation

- **Zero Build Warnings** - Strict code quality standards
  - StyleCop.Analyzers compliance
  - SonarAnalyzer.CSharp compliance
  - SecurityCodeScan.VS2019 compliance
  - All warnings treated as errors

- **FluentAssertions** - Modern, readable test assertions throughout

### Documentation

- **Comprehensive README Files**
  - Root README with quick start and feature comparison
  - TUnit.AutoFixture README with detailed usage guide
  - TUnit.AutoFixture.NSubstitute README with mocking examples

- **XML Documentation** - Complete IntelliSense support
  - All public APIs documented
  - Usage examples in XML comments
  - Clear descriptions of behavior and parameters

- **Example Tests** - Production-ready examples
  - BasicAutoDataTests.cs - Primitive and complex type generation
  - FrozenDependencyTests.cs - All matching strategies demonstrated
  - InlineAutoDataTests.cs - Hybrid explicit/auto data patterns
  - AutoNSubstituteTests.cs - Complete mocking scenarios

### Dependencies

#### TUnit.AutoFixture
- TUnit.Core 1.10.0
- AutoFixture 4.18.1
- .NET 10.0

#### TUnit.AutoFixture.NSubstitute
- TUnit.AutoFixture 1.0.0
- AutoFixture.AutoNSubstitute 4.18.1
- NSubstitute 5.1.0
- .NET 10.0

### Known Limitations

- **TUnit Null Literal Handling** - TUnit's attribute mechanism does not preserve `null` literal values passed to `[InlineAutoData(null!)]`. This is a TUnit framework limitation, not a limitation of this library.

- **Frozen Type Scope** - When a type is frozen, AutoFixture freezes it for the entire fixture instance. All subsequent requests for that type return the same frozen instance. This is standard AutoFixture behavior.

### Migration Guides

#### From xUnit.AutoFixture

**Minimal changes required:**
1. Change `[Theory]` to `[Test]`
2. Update using statements from `AutoFixture.Xunit2` to `TUnit.AutoFixture`
3. All attribute names remain identical

#### From Official AutoFixture.TUnit

**Attribute name changes:**
- `[AutoDataSource]` → `[AutoData]`
- `[AutoArguments]` → `[InlineAutoData]`
- All other usage remains identical

### Comparison with Official AutoFixture.TUnit

| Aspect | Official Implementation | TUnit.AutoFixture |
|--------|-------------------------|-------------------|
| **Internal Classes** | 20+ classes | 6-8 classes |
| **Primary Attribute** | `AutoDataSourceAttribute` | `AutoDataAttribute` |
| **Inline Attribute** | `AutoArgumentsAttribute` | `InlineAutoDataAttribute` |
| **xUnit Compatibility** | Different naming | Identical naming |
| **Architecture** | Multiple base classes | Direct TUnit integration |
| **Feature Parity** | ✅ Complete | ✅ Complete |

### Installation

**NuGet Package Manager:**
```powershell
Install-Package TUnit.AutoFixture
Install-Package TUnit.AutoFixture.NSubstitute
```

**dotnet CLI:**
```bash
dotnet add package TUnit.AutoFixture
dotnet add package TUnit.AutoFixture.NSubstitute
```

### Quick Start Example

```csharp
using TUnit.AutoFixture;
using TUnit.AutoFixture.NSubstitute;
using TUnit.Core;
using NSubstitute;
using FluentAssertions;

public class QuickStartTests
{
    [Test]
    [AutoData]
    public void BasicAutoData_Test(string text, int number, Person person)
    {
        text.Should().NotBeNull();
        number.Should().NotBe(0);
        person.Should().NotBeNull();
    }

    [Test]
    [InlineAutoData("explicit", 42)]
    public void InlineAutoData_Test(string explicitText, int explicitNumber, Person autoPerson)
    {
        explicitText.Should().Be("explicit");
        explicitNumber.Should().Be(42);
        autoPerson.Should().NotBeNull();
    }

    [Test]
    [AutoNSubstituteData]
    public void AutoNSubstitute_Test([Frozen] IService service, Consumer sut)
    {
        service.GetData().Returns("test");
        var result = sut.Process();
        result.Should().Be("test");
        service.Received(1).GetData();
    }
}
```

### Contributors

Special thanks to:
- [AutoFixture](https://github.com/AutoFixture/AutoFixture) - The excellent fixture library
- [TUnit](https://github.com/thomhurst/TUnit) - Modern .NET testing framework
- [Official AutoFixture.TUnit](https://github.com/AutoFixture/AutoFixture.TUnit) - Reference implementation

### License

This project is licensed under the MIT License.

### What's Next

Planned for future releases:
- Additional customization attributes
- Performance optimizations
- Extended documentation with video tutorials
- Additional auto-mocking library integrations (Moq, FakeItEasy)

---

For detailed usage documentation, see:
- [TUnit.AutoFixture README](TUnit.AutoFixture/README.md)
- [TUnit.AutoFixture.NSubstitute README](TUnit.AutoFixture.NSubstitute/README.md)
