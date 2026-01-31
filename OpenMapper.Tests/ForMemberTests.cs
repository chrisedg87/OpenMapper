using OpenMapper.Configuration;
using OpenMapper.Core;

namespace OpenMapper.Tests;

public class ForMemberTests
{
    [Fact]
    public void ForMember_WithComplexExpressions_ShouldMapCorrectly()
    {
        // Arrange
        var config = new MapperConfiguration(new ChapterProfile());
        var mapper = config.CreateMapper();
        var chapter = new Chapter
        {
            Lines = new List<string> { "Line 1", "Line 2", "Line 3" },
            PhotoUrl = "https://example.com/photo.jpg",
            Translations = new List<Translation>
            {
                new Translation { Language = "en", Text = "Hello" },
                new Translation { Language = "es", Text = "Hola" }
            }
        };

        // Act
        var dto = mapper.Map<Chapter, ChapterDto>(chapter);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(3, dto.NumberOfLines);
        Assert.Equal("https://example.com/photo.jpg", dto.CoverPhotoUrl);
        Assert.NotNull(dto.Translation);
        Assert.Equal("en", dto.Translation.Language);
        Assert.Equal("Hello", dto.Translation.Text);
    }

    [Fact]
    public void ForMember_WithTypeConversion_ShouldConvertCorrectly()
    {
        // Arrange
        var config = new MapperConfiguration(new ProductProfile());
        var mapper = config.CreateMapper();
        var product = new Product
        {
            Name = "Laptop",
            Price = 999.99m,
            Stock = 5
        };

        // Act
        var dto = mapper.Map<Product, ProductDto>(product);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal("Laptop", dto.Name); // Auto-mapped
        Assert.Equal("999.99", dto.PriceString); // Custom mapped with conversion
        Assert.True(dto.InStock); // Custom mapped with boolean expression
    }

    [Fact]
    public void ForMember_WithZeroStock_ShouldReturnFalse()
    {
        // Arrange
        var config = new MapperConfiguration(new ProductProfile());
        var mapper = config.CreateMapper();
        var product = new Product
        {
            Name = "Out of Stock Item",
            Price = 50m,
            Stock = 0
        };

        // Act
        var dto = mapper.Map<Product, ProductDto>(product);

        // Assert
        Assert.NotNull(dto);
        Assert.False(dto.InStock);
    }

    [Fact]
    public void ForMember_WithMixedAutoAndCustomMapping_ShouldMapBothCorrectly()
    {
        // Arrange
        var config = new MapperConfiguration(new PersonWithForMemberProfile());
        var mapper = config.CreateMapper();
        var person = new Person
        {
            FirstName = "John",
            LastName = "Doe",
            Age = 30,
            Email = "JOHN.DOE@EXAMPLE.COM"
        };

        // Act
        var dto = mapper.Map<Person, PersonDto>(person);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal("John", dto.FirstName); // Auto-mapped
        Assert.Equal("Doe", dto.LastName); // Auto-mapped
        Assert.Equal(30, dto.Age); // Auto-mapped
        Assert.Equal("john.doe@example.com", dto.Email); // Custom mapped with ToLower()
    }

    [Fact]
    public void ForMember_WithNullSourceValue_ShouldHandleGracefully()
    {
        // Arrange
        var config = new MapperConfiguration(new ChapterProfile());
        var mapper = config.CreateMapper();
        var chapter = new Chapter
        {
            Lines = new List<string> { "Line 1" },
            PhotoUrl = "photo.jpg",
            Translations = new List<Translation>() // Empty list
        };

        // Act
        var dto = mapper.Map<Chapter, ChapterDto>(chapter);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(1, dto.NumberOfLines);
        Assert.Equal("photo.jpg", dto.CoverPhotoUrl);
        Assert.Null(dto.Translation); // FirstOrDefault on empty list returns null
    }

    [Fact]
    public void ForMember_ToExistingInstance_ShouldApplyCustomMapping()
    {
        // Arrange
        var config = new MapperConfiguration(new ProductProfile());
        var mapper = config.CreateMapper();
        var product = new Product
        {
            Name = "Mouse",
            Price = 25.50m,
            Stock = 10
        };
        var existingDto = new ProductDto
        {
            Name = "Old Product",
            PriceString = "0",
            InStock = false
        };

        // Act
        var result = mapper.Map(product, existingDto);

        // Assert
        Assert.Same(existingDto, result);
        Assert.Equal("Mouse", result.Name);
        Assert.Equal("25.50", result.PriceString); // Custom conversion applied (decimal.ToString() keeps trailing zeros)
        Assert.True(result.InStock); // Custom expression applied
    }

    [Fact]
    public void ForMember_WithMultipleChainedCalls_ShouldApplyAllMappings()
    {
        // Arrange - ChapterProfile chains multiple ForMember calls
        var config = new MapperConfiguration(new ChapterProfile());
        var mapper = config.CreateMapper();
        var chapter = new Chapter
        {
            Lines = new List<string> { "A", "B" },
            PhotoUrl = "test.jpg",
            Translations = new List<Translation>
            {
                new Translation { Language = "fr", Text = "Bonjour" }
            }
        };

        // Act
        var dto = mapper.Map<Chapter, ChapterDto>(chapter);

        // Assert - All three ForMember configurations should be applied
        Assert.Equal(2, dto.NumberOfLines);
        Assert.Equal("test.jpg", dto.CoverPhotoUrl);
        Assert.Equal("fr", dto.Translation?.Language);
    }

    // Note: Cannot test ForMember with non-existent property because:
    // 1. Expression<Func<TDestination, TMember>> won't compile if property doesn't exist
    // 2. TypeMapConfiguration and MemberConfiguration are internal, can't manually construct
    // The validation logic in TypeMap.BuildPropertyMaps() will throw InvalidOperationException
    // if a ForMember configuration references a non-existent property, but we can't write
    // a unit test for it without either making internal classes public or using reflection hacks.

    [Fact]
    public void ForMember_BackwardCompatibility_ExistingTestsShouldStillPass()
    {
        // Arrange - Use old-style mapping without ForMember
        var config = new MapperConfiguration(new PersonProfile());
        var mapper = config.CreateMapper();
        var person = new Person
        {
            FirstName = "Test",
            LastName = "User",
            Age = 25,
            Email = "test@example.com"
        };

        // Act
        var dto = mapper.Map<Person, PersonDto>(person);

        // Assert - Should work exactly as before
        Assert.NotNull(dto);
        Assert.Equal("Test", dto.FirstName);
        Assert.Equal("User", dto.LastName);
        Assert.Equal(25, dto.Age);
        Assert.Equal("test@example.com", dto.Email);
    }

    [Fact]
    public void ForMember_WithPropertyChaining_ShouldWork()
    {
        // Arrange
        var profile = new TestPropertyChainingProfile();
        var config = new MapperConfiguration(profile);
        var mapper = config.CreateMapper();

        var personWithAddress = new PersonWithAddress
        {
            Name = "John",
            Address = new Address { City = "New York" }
        };

        // Act
        var dto = mapper.Map<PersonWithAddress, PersonCityDto>(personWithAddress);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal("John", dto.Name);
        Assert.Equal("New York", dto.City);
    }

    [Fact]
    public void ForMember_WithNullChaining_ShouldHandleNull()
    {
        // Arrange
        var profile = new TestPropertyChainingProfile();
        var config = new MapperConfiguration(profile);
        var mapper = config.CreateMapper();

        var personWithAddress = new PersonWithAddress
        {
            Name = "John",
            Address = null // Null address
        };

        // Act & Assert
        // This will throw NullReferenceException because we're accessing Address.City on null
        // This is expected behavior - user needs to handle null in their expression
        Assert.Throws<NullReferenceException>(() =>
            mapper.Map<PersonWithAddress, PersonCityDto>(personWithAddress));
    }
}

// Test helper profiles defined inline
public class PersonCityDto
{
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
}

public class TestPropertyChainingProfile : Profile
{
    public TestPropertyChainingProfile()
    {
        CreateMap<PersonWithAddress, PersonCityDto>()
            .ForMember(m => m.City, opt => opt.MapFrom(src => src.Address!.City));
    }
}

public class InvalidDestinationModel
{
    public string FirstName { get; set; } = string.Empty;
}
