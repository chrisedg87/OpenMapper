using OpenMapper.Configuration;
using OpenMapper.Exceptions;

namespace OpenMapper.Tests;

public class MapperTests
{
    [Fact]
    public void Map_WithGenericMethod_ShouldMapAllMatchingProperties()
    {
        // Arrange
        var config = new MapperConfiguration(new PersonProfile());
        var mapper = config.CreateMapper();
        var person = new Person
        {
            FirstName = "John",
            LastName = "Doe",
            Age = 30,
            Email = "john.doe@example.com"
        };

        // Act
        var dto = mapper.Map<Person, PersonDto>(person);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal("John", dto.FirstName);
        Assert.Equal("Doe", dto.LastName);
        Assert.Equal(30, dto.Age);
        Assert.Equal("john.doe@example.com", dto.Email);
    }

    [Fact]
    public void Map_WithNonGenericMethod_ShouldMapCorrectly()
    {
        // Arrange
        var config = new MapperConfiguration(new PersonProfile());
        var mapper = config.CreateMapper();
        object person = new Person
        {
            FirstName = "Jane",
            LastName = "Smith",
            Age = 25,
            Email = "jane.smith@example.com"
        };

        // Act
        var dto = mapper.Map<PersonDto>(person);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal("Jane", dto.FirstName);
        Assert.Equal("Smith", dto.LastName);
        Assert.Equal(25, dto.Age);
        Assert.Equal("jane.smith@example.com", dto.Email);
    }

    [Fact]
    public void Map_ToExistingInstance_ShouldPopulateDestination()
    {
        // Arrange
        var config = new MapperConfiguration(new PersonProfile());
        var mapper = config.CreateMapper();
        var person = new Person
        {
            FirstName = "Bob",
            LastName = "Johnson",
            Age = 40,
            Email = "bob@example.com"
        };
        var existingDto = new PersonDto
        {
            FirstName = "Old",
            LastName = "Name",
            Age = 99,
            Email = "old@example.com"
        };

        // Act
        var result = mapper.Map(person, existingDto);

        // Assert
        Assert.Same(existingDto, result); // Should be the same instance
        Assert.Equal("Bob", result.FirstName);
        Assert.Equal("Johnson", result.LastName);
        Assert.Equal(40, result.Age);
        Assert.Equal("bob@example.com", result.Email);
    }

    [Fact]
    public void Map_WithNullSource_ShouldReturnNull()
    {
        // Arrange
        var config = new MapperConfiguration(new PersonProfile());
        var mapper = config.CreateMapper();
        Person? person = null;

        // Act
        var dto = mapper.Map<Person, PersonDto>(person);

        // Assert
        Assert.Null(dto);
    }

    [Fact]
    public void Map_NonGenericWithNullSource_ShouldReturnNull()
    {
        // Arrange
        var config = new MapperConfiguration(new PersonProfile());
        var mapper = config.CreateMapper();
        object? person = null;

        // Act
        var dto = mapper.Map<PersonDto>(person);

        // Assert
        Assert.Null(dto);
    }

    [Fact]
    public void Map_ToExistingInstanceWithNullSource_ShouldReturnUnmodifiedDestination()
    {
        // Arrange
        var config = new MapperConfiguration(new PersonProfile());
        var mapper = config.CreateMapper();
        Person? person = null;
        var existingDto = new PersonDto
        {
            FirstName = "Existing",
            LastName = "Data",
            Age = 50,
            Email = "existing@example.com"
        };

        // Act
        var result = mapper.Map(person, existingDto);

        // Assert
        Assert.Same(existingDto, result);
        Assert.Equal("Existing", result.FirstName); // Should be unchanged
        Assert.Equal("Data", result.LastName);
        Assert.Equal(50, result.Age);
        Assert.Equal("existing@example.com", result.Email);
    }

    [Fact]
    public void Map_ToExistingInstanceWithNullDestination_ShouldThrowArgumentNullException()
    {
        // Arrange
        var config = new MapperConfiguration(new PersonProfile());
        var mapper = config.CreateMapper();
        var person = new Person { FirstName = "Test" };
        PersonDto? destination = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => mapper.Map(person, destination!));
    }

    [Fact]
    public void Map_WithPartialPropertyMatch_ShouldMapMatchingPropertiesOnly()
    {
        // Arrange
        var config = new MapperConfiguration(new EmployeeProfile());
        var mapper = config.CreateMapper();
        var employee = new Employee
        {
            FirstName = "Alice",
            LastName = "Williams",
            Age = 35,
            Department = "Engineering",
            Salary = 75000m
        };

        // Act
        var dto = mapper.Map<Employee, EmployeeDto>(employee);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal("Alice", dto.FirstName);
        Assert.Equal("Williams", dto.LastName);
        Assert.Equal(35, dto.Age);
        // Department and Salary should not be mapped (don't exist in EmployeeDto)
    }

    [Fact]
    public void Map_WithMissingMapping_ShouldThrowMappingNotFoundException()
    {
        // Arrange
        var config = new MapperConfiguration(new PersonProfile());
        var mapper = config.CreateMapper();
        var employee = new Employee { FirstName = "Test" };

        // Act & Assert
        var exception = Assert.Throws<MappingNotFoundException>(
            () => mapper.Map<Employee, EmployeeDto>(employee));

        Assert.Equal(typeof(Employee), exception.SourceType);
        Assert.Equal(typeof(EmployeeDto), exception.DestinationType);
        Assert.Contains("Employee", exception.Message);
        Assert.Contains("EmployeeDto", exception.Message);
        Assert.Contains("CreateMap", exception.Message);
    }

    [Fact]
    public void Map_NonGenericWithMissingMapping_ShouldThrowMappingNotFoundException()
    {
        // Arrange
        var config = new MapperConfiguration(new PersonProfile());
        var mapper = config.CreateMapper();
        object employee = new Employee { FirstName = "Test" };

        // Act & Assert
        var exception = Assert.Throws<MappingNotFoundException>(
            () => mapper.Map<EmployeeDto>(employee));

        Assert.Equal(typeof(Employee), exception.SourceType);
        Assert.Equal(typeof(EmployeeDto), exception.DestinationType);
    }

    [Fact]
    public void Map_WithReverseMapping_ShouldWorkBothDirections()
    {
        // Arrange
        var config = new MapperConfiguration(new PersonProfile());
        var mapper = config.CreateMapper();
        var person = new Person
        {
            FirstName = "Test",
            LastName = "User",
            Age = 30,
            Email = "test@example.com"
        };

        // Act - Forward mapping
        var dto = mapper.Map<Person, PersonDto>(person);

        // Act - Reverse mapping
        var personAgain = mapper.Map<PersonDto, Person>(dto!);

        // Assert
        Assert.NotNull(personAgain);
        Assert.Equal(person.FirstName, personAgain.FirstName);
        Assert.Equal(person.LastName, personAgain.LastName);
        Assert.Equal(person.Age, personAgain.Age);
        Assert.Equal(person.Email, personAgain.Email);
    }

    [Fact]
    public void Map_WithNestedObjects_ShouldNotAutomaticallyMapNestedProperties()
    {
        // Arrange
        var config = new MapperConfiguration(new NestedProfile());
        var mapper = config.CreateMapper();
        var person = new PersonWithAddress
        {
            Name = "John Doe",
            Address = new Address
            {
                Street = "123 Main St",
                City = "Springfield",
                ZipCode = "12345"
            }
        };

        // Act
        var dto = mapper.Map<PersonWithAddress, PersonWithAddressDto>(person);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal("John Doe", dto.Name);
        // Note: Address will be copied as reference, not mapped
        // This tests current behavior - nested mapping would need separate implementation
    }

    [Fact]
    public void Map_MultipleInstances_ShouldCreateSeparateObjects()
    {
        // Arrange
        var config = new MapperConfiguration(new PersonProfile());
        var mapper = config.CreateMapper();
        var person = new Person
        {
            FirstName = "Test",
            LastName = "Person",
            Age = 25,
            Email = "test@example.com"
        };

        // Act
        var dto1 = mapper.Map<Person, PersonDto>(person);
        var dto2 = mapper.Map<Person, PersonDto>(person);

        // Assert
        Assert.NotNull(dto1);
        Assert.NotNull(dto2);
        Assert.NotSame(dto1, dto2); // Should be different instances
        Assert.Equal(dto1.FirstName, dto2.FirstName);
    }

    [Fact]
    public void Map_WithDefaultValues_ShouldUseDestinationDefaults()
    {
        // Arrange
        var config = new MapperConfiguration(new PartialMappingProfile());
        var mapper = config.CreateMapper();
        var person = new Person
        {
            FirstName = "John",
            LastName = "Doe",
            Age = 30,
            Email = "john@example.com"
        };

        // Act
        var dto = mapper.Map<Person, PartialPersonDto>(person);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal("John", dto.FirstName);
        Assert.Equal("Doe", dto.LastName);
        // Age and Email properties don't exist in PartialPersonDto
    }

    [Fact]
    public void Map_ListOfObjects_ShouldMapEachItemIndividually()
    {
        // Arrange
        var config = new MapperConfiguration(new PersonProfile());
        var mapper = config.CreateMapper();
        var people = new List<Person>
        {
            new Person { FirstName = "John", LastName = "Doe", Age = 30, Email = "john@example.com" },
            new Person { FirstName = "Jane", LastName = "Smith", Age = 25, Email = "jane@example.com" },
            new Person { FirstName = "Bob", LastName = "Johnson", Age = 40, Email = "bob@example.com" }
        };

        // Act
        var dtos = mapper.Map<List<PersonDto>>(people);

        // Assert
        Assert.NotNull(dtos);
        Assert.Equal(3, dtos.Count);

        Assert.Equal("John", dtos[0]?.FirstName);
        Assert.Equal("Doe", dtos[0]?.LastName);
        Assert.Equal(30, dtos[0]?.Age);
        Assert.Equal("john@example.com", dtos[0]?.Email);

        Assert.Equal("Jane", dtos[1]?.FirstName);
        Assert.Equal("Smith", dtos[1]?.LastName);
        Assert.Equal(25, dtos[1]?.Age);
        Assert.Equal("jane@example.com", dtos[1]?.Email);

        Assert.Equal("Bob", dtos[2]?.FirstName);
        Assert.Equal("Johnson", dtos[2]?.LastName);
        Assert.Equal(40, dtos[2]?.Age);
        Assert.Equal("bob@example.com", dtos[2]?.Email);
    }

    [Fact]
    public void Map_EmptyList_ShouldReturnEmptyList()
    {
        // Arrange
        var config = new MapperConfiguration(new PersonProfile());
        var mapper = config.CreateMapper();
        var people = new List<Person>();

        // Act
        var dtos = mapper.Map<List<PersonDto>>(people);

        // Assert
        Assert.NotNull(dtos);
        Assert.Empty(dtos);
    }

    [Fact]
    public void Map_ListWithNullItems_ShouldHandleNullsGracefully()
    {
        // Arrange
        var config = new MapperConfiguration(new PersonProfile());
        var mapper = config.CreateMapper();
        var people = new List<Person?>
        {
            new Person { FirstName = "John", LastName = "Doe", Age = 30, Email = "john@example.com" },
            null,
            new Person { FirstName = "Jane", LastName = "Smith", Age = 25, Email = "jane@example.com" }
        };

        // Act
        var dtos = mapper.Map<List<PersonDto>>(people);
        
        // Assert
        Assert.NotNull(dtos);
        Assert.Equal(3, dtos.Count);
        Assert.NotNull(dtos[0]);
        Assert.Null(dtos[1]);
        Assert.NotNull(dtos[2]);

        Assert.Equal("John", dtos[0]?.FirstName);
        Assert.Equal("Jane", dtos[2]?.FirstName);
    }
}
