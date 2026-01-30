using OpenMapper.Configuration;
using OpenMapper.Exceptions;

namespace OpenMapper.Tests;

public class ExceptionTests
{
    [Fact]
    public void MappingNotFoundException_ShouldContainSourceType()
    {
        // Arrange
        var sourceType = typeof(Person);
        var destinationType = typeof(EmployeeDto);

        // Act
        var exception = new MappingNotFoundException(sourceType, destinationType);

        // Assert
        Assert.Equal(sourceType, exception.SourceType);
    }

    [Fact]
    public void MappingNotFoundException_ShouldContainDestinationType()
    {
        // Arrange
        var sourceType = typeof(Person);
        var destinationType = typeof(EmployeeDto);

        // Act
        var exception = new MappingNotFoundException(sourceType, destinationType);

        // Assert
        Assert.Equal(destinationType, exception.DestinationType);
    }

    [Fact]
    public void MappingNotFoundException_MessageShouldContainTypeNames()
    {
        // Arrange
        var sourceType = typeof(Person);
        var destinationType = typeof(EmployeeDto);

        // Act
        var exception = new MappingNotFoundException(sourceType, destinationType);

        // Assert
        Assert.Contains("Person", exception.Message);
        Assert.Contains("EmployeeDto", exception.Message);
    }

    [Fact]
    public void MappingNotFoundException_MessageShouldContainHelpfulGuidance()
    {
        // Arrange
        var sourceType = typeof(Person);
        var destinationType = typeof(EmployeeDto);

        // Act
        var exception = new MappingNotFoundException(sourceType, destinationType);

        // Assert
        Assert.Contains("CreateMap", exception.Message);
        Assert.Contains("Profile", exception.Message);
    }

    [Fact]
    public void MappingNotFoundException_ShouldBeThrowableAndCatchable()
    {
        // Arrange
        var config = new MapperConfiguration(new PersonProfile());
        var mapper = config.CreateMapper();
        var employee = new Employee { FirstName = "Test" };

        // Act & Assert
        var exception = Assert.Throws<MappingNotFoundException>(() =>
        {
            mapper.Map<Employee, EmployeeDto>(employee);
        });

        Assert.NotNull(exception);
        Assert.IsType<MappingNotFoundException>(exception);
    }

    [Fact]
    public void MappingNotFoundException_ShouldInheritFromOpenMapperException()
    {
        // Arrange
        var sourceType = typeof(Person);
        var destinationType = typeof(EmployeeDto);

        // Act
        var exception = new MappingNotFoundException(sourceType, destinationType);

        // Assert
        Assert.IsAssignableFrom<OpenMapperException>(exception);
    }

    [Fact]
    public void OpenMapperException_ShouldBeBaseException()
    {
        // Arrange & Act
        var exception = new OpenMapperException("Test message");

        // Assert
        Assert.IsAssignableFrom<Exception>(exception);
        Assert.Equal("Test message", exception.Message);
    }

    [Fact]
    public void MappingNotFoundException_CanBeCaughtAsOpenMapperException()
    {
        // Arrange
        var config = new MapperConfiguration(new PersonProfile());
        var mapper = config.CreateMapper();
        var employee = new Employee { FirstName = "Test" };

        // Act
        OpenMapperException? caughtException = null;
        try
        {
            mapper.Map<Employee, EmployeeDto>(employee);
        }
        catch (OpenMapperException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.NotNull(caughtException);
        Assert.IsType<MappingNotFoundException>(caughtException);
    }

    [Fact]
    public void MappingNotFoundException_ThrownByGenericMap_ShouldHaveCorrectTypes()
    {
        // Arrange
        var config = new MapperConfiguration(new PersonProfile());
        var mapper = config.CreateMapper();
        var address = new Address { Street = "Test St" };

        // Act
        var exception = Assert.Throws<MappingNotFoundException>(() =>
        {
            mapper.Map<Address, AddressDto>(address);
        });

        // Assert
        Assert.Equal(typeof(Address), exception.SourceType);
        Assert.Equal(typeof(AddressDto), exception.DestinationType);
    }

    [Fact]
    public void MappingNotFoundException_ThrownByNonGenericMap_ShouldHaveCorrectTypes()
    {
        // Arrange
        var config = new MapperConfiguration(new PersonProfile());
        var mapper = config.CreateMapper();
        object address = new Address { Street = "Test St" };

        // Act
        var exception = Assert.Throws<MappingNotFoundException>(() =>
        {
            mapper.Map<AddressDto>(address);
        });

        // Assert
        Assert.Equal(typeof(Address), exception.SourceType);
        Assert.Equal(typeof(AddressDto), exception.DestinationType);
    }

    [Fact]
    public void MappingNotFoundException_ThrownByMapToExisting_ShouldHaveCorrectTypes()
    {
        // Arrange
        var config = new MapperConfiguration(new PersonProfile());
        var mapper = config.CreateMapper();
        var address = new Address { Street = "Test St" };
        var addressDto = new AddressDto();

        // Act
        var exception = Assert.Throws<MappingNotFoundException>(() =>
        {
            mapper.Map(address, addressDto);
        });

        // Assert
        Assert.Equal(typeof(Address), exception.SourceType);
        Assert.Equal(typeof(AddressDto), exception.DestinationType);
    }
}
