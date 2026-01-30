using OpenMapper.Configuration;
using OpenMapper.Core;

namespace OpenMapper.Tests;

public class MapperConfigurationTests
{
    [Fact]
    public void Constructor_WithSingleProfile_ShouldCreateConfiguration()
    {
        // Arrange & Act
        var config = new MapperConfiguration(new PersonProfile());
        var mapper = config.CreateMapper();

        // Assert
        Assert.NotNull(config);
        Assert.NotNull(mapper);
    }

    [Fact]
    public void Constructor_WithMultipleProfiles_ShouldIncludeAllMappings()
    {
        // Arrange & Act
        var config = new MapperConfiguration(new PersonProfile(), new EmployeeProfile());
        var mapper = config.CreateMapper();

        // Test Person mapping
        var person = new Person { FirstName = "John", LastName = "Doe", Age = 30, Email = "john@example.com" };
        var personDto = mapper.Map<Person, PersonDto>(person);

        // Test Employee mapping
        var employee = new Employee { FirstName = "Jane", LastName = "Smith", Age = 25 };
        var employeeDto = mapper.Map<Employee, EmployeeDto>(employee);

        // Assert
        Assert.NotNull(personDto);
        Assert.Equal("John", personDto.FirstName);
        Assert.NotNull(employeeDto);
        Assert.Equal("Jane", employeeDto.FirstName);
    }

    [Fact]
    public void Constructor_WithEmptyProfile_ShouldNotThrow()
    {
        // Arrange & Act
        var config = new MapperConfiguration(new EmptyProfile());
        var mapper = config.CreateMapper();

        // Assert
        Assert.NotNull(config);
        Assert.NotNull(mapper);
    }

    [Fact]
    public void Constructor_WithNoProfiles_ShouldCreateEmptyConfiguration()
    {
        // Arrange & Act
        var config = new MapperConfiguration();
        var mapper = config.CreateMapper();

        // Assert
        Assert.NotNull(config);
        Assert.NotNull(mapper);
    }

    [Fact]
    public void CreateMapper_ShouldReturnNewMapperInstance()
    {
        // Arrange
        var config = new MapperConfiguration(new PersonProfile());

        // Act
        var mapper1 = config.CreateMapper();
        var mapper2 = config.CreateMapper();

        // Assert
        Assert.NotNull(mapper1);
        Assert.NotNull(mapper2);
        Assert.NotSame(mapper1, mapper2);
    }

    [Fact]
    public void CreateMapper_MultipleCalls_ShouldShareSameConfiguration()
    {
        // Arrange
        var config = new MapperConfiguration(new PersonProfile());
        var person = new Person
        {
            FirstName = "Test",
            LastName = "User",
            Age = 30,
            Email = "test@example.com"
        };

        // Act
        var mapper1 = config.CreateMapper();
        var mapper2 = config.CreateMapper();
        var dto1 = mapper1.Map<Person, PersonDto>(person);
        var dto2 = mapper2.Map<Person, PersonDto>(person);

        // Assert
        Assert.Equal(dto1.FirstName, dto2.FirstName);
        Assert.Equal(dto1.LastName, dto2.LastName);
        Assert.Equal(dto1.Age, dto2.Age);
        Assert.Equal(dto1.Email, dto2.Email);
    }

    [Fact]
    public void Constructor_WithDuplicateMapping_ShouldUseLastMapping()
    {
        // Arrange
        var profile1 = new PersonProfile();
        var profile2 = new PersonProfile(); // Same mappings defined

        // Act
        var config = new MapperConfiguration(profile1, profile2);
        var mapper = config.CreateMapper();
        var person = new Person { FirstName = "Test", LastName = "User", Age = 30, Email = "test@example.com" };
        var dto = mapper.Map<Person, PersonDto>(person);

        // Assert - Should not throw and should map correctly
        Assert.NotNull(dto);
        Assert.Equal("Test", dto.FirstName);
    }

    [Fact]
    public void MapperConfiguration_WithMixedProfiles_ShouldHandleAllMappings()
    {
        // Arrange
        var config = new MapperConfiguration(
            new PersonProfile(),
            new EmployeeProfile(),
            new AddressProfile(),
            new EmptyProfile()
        );
        var mapper = config.CreateMapper();

        // Act & Assert - Person mapping
        var person = new Person { FirstName = "John", LastName = "Doe", Age = 30, Email = "john@example.com" };
        var personDto = mapper.Map<Person, PersonDto>(person);
        Assert.NotNull(personDto);
        Assert.Equal("John", personDto.FirstName);

        // Act & Assert - Employee mapping
        var employee = new Employee { FirstName = "Jane", LastName = "Smith", Age = 25 };
        var employeeDto = mapper.Map<Employee, EmployeeDto>(employee);
        Assert.NotNull(employeeDto);
        Assert.Equal("Jane", employeeDto.FirstName);

        // Act & Assert - Address mapping
        var address = new Address { Street = "123 Main St", City = "Springfield", ZipCode = "12345" };
        var addressDto = mapper.Map<Address, AddressDto>(address);
        Assert.NotNull(addressDto);
        Assert.Equal("123 Main St", addressDto.Street);
    }
}
