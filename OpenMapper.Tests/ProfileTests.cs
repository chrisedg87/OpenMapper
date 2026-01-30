using OpenMapper.Configuration;
using OpenMapper.Core;

namespace OpenMapper.Tests;

public class ProfileTests
{
    [Fact]
    public void CreateMap_ShouldEnableMapping()
    {
        // Arrange & Act
        var config = new MapperConfiguration(new PersonProfile());
        var mapper = config.CreateMapper();
        var person = new Person
        {
            FirstName = "Test",
            LastName = "User",
            Age = 30,
            Email = "test@example.com"
        };

        var dto = mapper.Map<Person, PersonDto>(person);

        // Assert - If CreateMap worked, mapping should succeed
        Assert.NotNull(dto);
        Assert.Equal("Test", dto.FirstName);
    }

    [Fact]
    public void CreateMap_ShouldReturnMappingExpression()
    {
        // Arrange
        var profile = new TestProfile();

        // Act
        var expression = profile.CreateMapPublic<Person, PersonDto>();

        // Assert
        Assert.NotNull(expression);
    }

    [Fact]
    public void CreateMap_MultipleTypes_ShouldEnableAllMappings()
    {
        // Arrange
        var config = new MapperConfiguration(new MultiTypeProfile());
        var mapper = config.CreateMapper();

        // Act & Assert - All three mappings should work
        var person = new Person { FirstName = "John", LastName = "Doe", Age = 30, Email = "john@example.com" };
        var personDto = mapper.Map<Person, PersonDto>(person);
        Assert.NotNull(personDto);

        var employee = new Employee { FirstName = "Jane", LastName = "Smith", Age = 25 };
        var employeeDto = mapper.Map<Employee, EmployeeDto>(employee);
        Assert.NotNull(employeeDto);

        var address = new Address { Street = "123 Main St", City = "Test City", ZipCode = "12345" };
        var addressDto = mapper.Map<Address, AddressDto>(address);
        Assert.NotNull(addressDto);
    }

    [Fact]
    public void EmptyProfile_ShouldNotCauseErrors()
    {
        // Arrange & Act
        var config = new MapperConfiguration(new EmptyProfile());
        var mapper = config.CreateMapper();

        // Assert
        Assert.NotNull(config);
        Assert.NotNull(mapper);
    }

    // Helper profile for testing CreateMap access
    private class TestProfile : Profile
    {
        public IMappingExpression<TSource, TDestination> CreateMapPublic<TSource, TDestination>()
        {
            return CreateMap<TSource, TDestination>();
        }
    }

    // Profile with multiple type mappings
    private class MultiTypeProfile : Profile
    {
        public MultiTypeProfile()
        {
            CreateMap<Person, PersonDto>();
            CreateMap<Employee, EmployeeDto>();
            CreateMap<Address, AddressDto>();
        }
    }
}
