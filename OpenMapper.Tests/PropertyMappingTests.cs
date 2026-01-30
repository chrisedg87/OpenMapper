using OpenMapper.Configuration;
using OpenMapper.Core;

namespace OpenMapper.Tests;

public class PropertyMappingTests
{
    [Fact]
    public void Map_WithMatchingPropertyNames_ShouldMapValues()
    {
        // Arrange
        var profile = new TestProfile();
        var config = new MapperConfiguration(profile);
        var mapper = config.CreateMapper();
        var source = new SourceModel
        {
            Name = "Test Name",
            Value = 42,
            Description = "Test Description"
        };

        // Act
        var destination = mapper.Map<SourceModel, DestinationModel>(source);

        // Assert
        Assert.NotNull(destination);
        Assert.Equal("Test Name", destination.Name);
        Assert.Equal(42, destination.Value);
        Assert.Equal("Test Description", destination.Description);
    }

    [Fact]
    public void Map_WithNonMatchingPropertyNames_ShouldIgnoreUnmatchedProperties()
    {
        // Arrange
        var profile = new TestProfile();
        var config = new MapperConfiguration(profile);
        var mapper = config.CreateMapper();
        var source = new SourceWithExtra
        {
            Name = "Test",
            ExtraProperty = "Should not be mapped"
        };

        // Act
        var destination = mapper.Map<SourceWithExtra, DestinationBasic>(source);

        // Assert
        Assert.NotNull(destination);
        Assert.Equal("Test", destination.Name);
        // ExtraProperty should be ignored
    }

    [Fact]
    public void Map_WithMissingSourceProperty_ShouldLeaveDestinationAsDefault()
    {
        // Arrange
        var profile = new TestProfile();
        var config = new MapperConfiguration(profile);
        var mapper = config.CreateMapper();
        var source = new SourceBasic
        {
            Name = "Test"
        };

        // Act
        var destination = mapper.Map<SourceBasic, DestinationWithExtra>(source);

        // Assert
        Assert.NotNull(destination);
        Assert.Equal("Test", destination.Name);
        Assert.Equal(default(int), destination.ExtraValue); // Should be 0
    }

    [Fact]
    public void Map_WithNullPropertyValue_ShouldMapNull()
    {
        // Arrange
        var profile = new TestProfile();
        var config = new MapperConfiguration(profile);
        var mapper = config.CreateMapper();
        var source = new SourceWithNullable
        {
            Name = null,
            Value = null
        };

        // Act
        var destination = mapper.Map<SourceWithNullable, DestinationWithNullable>(source);

        // Assert
        Assert.NotNull(destination);
        Assert.Null(destination.Name);
        Assert.Null(destination.Value);
    }

    [Fact]
    public void Map_WithTypeCompatibility_ShouldMapDerivedTypes()
    {
        // Arrange
        var profile = new TestProfile();
        var config = new MapperConfiguration(profile);
        var mapper = config.CreateMapper();
        var source = new SourceWithDerived
        {
            BaseValue = "test"
        };

        // Act
        var destination = mapper.Map<SourceWithDerived, DestinationWithBase>(source);

        // Assert
        Assert.NotNull(destination);
        Assert.Equal("test", destination.BaseValue);
    }

    [Fact]
    public void Map_CaseSensitivePropertyNames_ShouldRequireExactMatch()
    {
        // Arrange
        var profile = new TestProfile();
        var config = new MapperConfiguration(profile);
        var mapper = config.CreateMapper();
        var source = new SourceCaseDifferent
        {
            name = "lowercase", // lowercase
            Value = 100
        };

        // Act
        var destination = mapper.Map<SourceCaseDifferent, DestinationCaseDifferent>(source);

        // Assert
        Assert.NotNull(destination);
        // "name" (lowercase) should not match "Name" (uppercase N)
        Assert.Null(destination.Name); // Should be default/null
        Assert.Equal(100, destination.Value);
    }

    [Fact]
    public void Map_WithPrivateSetters_ShouldMapToPrivateSetters()
    {
        // Arrange
        var profile = new TestProfile();
        var config = new MapperConfiguration(profile);
        var mapper = config.CreateMapper();
        var source = new SourceModel
        {
            Name = "Test",
            Value = 42
        };

        // Act
        var destination = mapper.Map<SourceModel, DestinationWithPrivateSetter>(source);

        // Assert
        Assert.NotNull(destination);
        // OpenMapper uses reflection and CAN map to private setters
        Assert.Equal("Test", destination.Name);
    }

    [Fact]
    public void Map_MultiplePropertiesDifferentTypes_ShouldMapCompatibleTypes()
    {
        // Arrange
        var profile = new TestProfile();
        var config = new MapperConfiguration(profile);
        var mapper = config.CreateMapper();
        var source = new ComplexSource
        {
            StringValue = "text",
            IntValue = 42,
            BoolValue = true,
            DoubleValue = 3.14
        };

        // Act
        var destination = mapper.Map<ComplexSource, ComplexDestination>(source);

        // Assert
        Assert.NotNull(destination);
        Assert.Equal("text", destination.StringValue);
        Assert.Equal(42, destination.IntValue);
        Assert.True(destination.BoolValue);
        Assert.Equal(3.14, destination.DoubleValue);
    }

    // Test models
    private class SourceModel
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    private class DestinationModel
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    private class SourceWithExtra
    {
        public string Name { get; set; } = string.Empty;
        public string ExtraProperty { get; set; } = string.Empty;
    }

    private class DestinationBasic
    {
        public string Name { get; set; } = string.Empty;
    }

    private class SourceBasic
    {
        public string Name { get; set; } = string.Empty;
    }

    private class DestinationWithExtra
    {
        public string Name { get; set; } = string.Empty;
        public int ExtraValue { get; set; }
    }

    private class SourceWithNullable
    {
        public string? Name { get; set; }
        public int? Value { get; set; }
    }

    private class DestinationWithNullable
    {
        public string? Name { get; set; }
        public int? Value { get; set; }
    }

    private class SourceWithDerived
    {
        public string BaseValue { get; set; } = string.Empty;
    }

    private class DestinationWithBase
    {
        public object? BaseValue { get; set; } // object can accept string
    }

    private class SourceCaseDifferent
    {
        public string name { get; set; } = string.Empty; // lowercase
        public int Value { get; set; }
    }

    private class DestinationCaseDifferent
    {
        public string? Name { get; set; } // uppercase N
        public int Value { get; set; }
    }

    private class DestinationWithPrivateSetter
    {
        public string Name { get; private set; } = string.Empty;
        public int Value { get; set; }
    }

    private class ComplexSource
    {
        public string StringValue { get; set; } = string.Empty;
        public int IntValue { get; set; }
        public bool BoolValue { get; set; }
        public double DoubleValue { get; set; }
    }

    private class ComplexDestination
    {
        public string StringValue { get; set; } = string.Empty;
        public int IntValue { get; set; }
        public bool BoolValue { get; set; }
        public double DoubleValue { get; set; }
    }

    // Test profile that maps all test types
    private class TestProfile : Profile
    {
        public TestProfile()
        {
            CreateMap<SourceModel, DestinationModel>();
            CreateMap<SourceWithExtra, DestinationBasic>();
            CreateMap<SourceBasic, DestinationWithExtra>();
            CreateMap<SourceWithNullable, DestinationWithNullable>();
            CreateMap<SourceWithDerived, DestinationWithBase>();
            CreateMap<SourceCaseDifferent, DestinationCaseDifferent>();
            CreateMap<SourceModel, DestinationWithPrivateSetter>();
            CreateMap<ComplexSource, ComplexDestination>();
        }
    }
}
