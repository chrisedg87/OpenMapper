using OpenMapper.Configuration;
using OpenMapper.Core;
using OpenMapper.Exceptions;

namespace OpenMapper.Tests;

public class ReverseMapTests
{
    // -------------------------------------------------------------------------
    // 1. Basic reverse mapping is registered
    // -------------------------------------------------------------------------

    [Fact]
    public void ReverseMap_ShouldRegisterReverseMapping_AllowingBothDirections()
    {
        var config = new MapperConfiguration(new PersonReverseProfile());
        var mapper = config.CreateMapper();

        var person = new Person { FirstName = "Alice", LastName = "Smith", Age = 30, Email = "alice@example.com" };
        var dto = mapper.Map<PersonDto>(person);
        var personBack = mapper.Map<Person>(dto!);

        Assert.NotNull(dto);
        Assert.NotNull(personBack);
    }

    // -------------------------------------------------------------------------
    // 2. Forward mapping still works after ReverseMap
    // -------------------------------------------------------------------------

    [Fact]
    public void ReverseMap_ForwardMapping_ShouldStillMapPropertiesCorrectly()
    {
        var config = new MapperConfiguration(new PersonReverseProfile());
        var mapper = config.CreateMapper();

        var person = new Person { FirstName = "Bob", LastName = "Jones", Age = 25, Email = "bob@test.com" };
        var dto = mapper.Map<Person, PersonDto>(person);

        Assert.NotNull(dto);
        Assert.Equal("Bob", dto.FirstName);
        Assert.Equal("Jones", dto.LastName);
        Assert.Equal(25, dto.Age);
        Assert.Equal("bob@test.com", dto.Email);
    }

    // -------------------------------------------------------------------------
    // 3. Reverse mapping maps properties correctly
    // -------------------------------------------------------------------------

    [Fact]
    public void ReverseMap_ReversedMapping_ShouldMapPropertiesCorrectly()
    {
        var config = new MapperConfiguration(new PersonReverseProfile());
        var mapper = config.CreateMapper();

        var dto = new PersonDto { FirstName = "Carol", LastName = "White", Age = 40, Email = "carol@test.com" };
        var person = mapper.Map<PersonDto, Person>(dto);

        Assert.NotNull(person);
        Assert.Equal("Carol", person.FirstName);
        Assert.Equal("White", person.LastName);
        Assert.Equal(40, person.Age);
        Assert.Equal("carol@test.com", person.Email);
    }

    // -------------------------------------------------------------------------
    // 4. Property values survive a full round-trip
    // -------------------------------------------------------------------------

    [Fact]
    public void ReverseMap_RoundTrip_ShouldPreserveAllMatchingPropertyValues()
    {
        var config = new MapperConfiguration(new PersonReverseProfile());
        var mapper = config.CreateMapper();

        var original = new Person { FirstName = "Dave", LastName = "Brown", Age = 35, Email = "dave@test.com" };
        var dto = mapper.Map<Person, PersonDto>(original);
        var roundTripped = mapper.Map<PersonDto, Person>(dto!);

        Assert.NotNull(roundTripped);
        Assert.Equal(original.FirstName, roundTripped.FirstName);
        Assert.Equal(original.LastName, roundTripped.LastName);
        Assert.Equal(original.Age, roundTripped.Age);
        Assert.Equal(original.Email, roundTripped.Email);
    }

    // -------------------------------------------------------------------------
    // 5. Reverse map with partial properties (destination has fewer members)
    // -------------------------------------------------------------------------

    [Fact]
    public void ReverseMap_PartialDestination_ShouldMapOnlyMatchingProperties()
    {
        var config = new MapperConfiguration(new EmployeeReverseProfile());
        var mapper = config.CreateMapper();

        var employeeDto = new EmployeeDto { FirstName = "Eve", LastName = "Taylor", Age = 28 };
        var employee = mapper.Map<EmployeeDto, Employee>(employeeDto);

        Assert.NotNull(employee);
        Assert.Equal("Eve", employee.FirstName);
        Assert.Equal("Taylor", employee.LastName);
        Assert.Equal(28, employee.Age);
        // Department and Salary have no corresponding EmployeeDto properties — stay default
        Assert.Equal(string.Empty, employee.Department);
        Assert.Equal(0m, employee.Salary);
    }

    // -------------------------------------------------------------------------
    // 6. ReverseMap returns an expression that allows ForMember on reverse direction
    // -------------------------------------------------------------------------

    [Fact]
    public void ReverseMap_WithForMemberOnReverse_ShouldApplyCustomTransform()
    {
        var config = new MapperConfiguration(new AddressReverseWithForMemberProfile());
        var mapper = config.CreateMapper();

        var dto = new AddressDto { Street = "10 Main St", City = "Springfield", ZipCode = "12345" };
        var address = mapper.Map<AddressDto, Address>(dto);

        Assert.NotNull(address);
        Assert.Equal("SPRINGFIELD", address.City); // ForMember applied ToUpper on reverse
        Assert.Equal("10 Main St", address.Street);
        Assert.Equal("12345", address.ZipCode);
    }

    // -------------------------------------------------------------------------
    // 7. Multiple CreateMap+ReverseMap — all four directions work
    // -------------------------------------------------------------------------

    [Fact]
    public void ReverseMap_MultipleCreateMapCalls_AllFourDirectionsShouldWork()
    {
        var config = new MapperConfiguration(new MultiReverseProfile());
        var mapper = config.CreateMapper();

        var person = new Person { FirstName = "Frank", LastName = "Green", Age = 22, Email = "frank@test.com" };
        var address = new Address { Street = "5 Elm St", City = "Shelbyville", ZipCode = "67890" };

        var personDto = mapper.Map<Person, PersonDto>(person);
        var personBack = mapper.Map<PersonDto, Person>(personDto!);
        var addressDto = mapper.Map<Address, AddressDto>(address);
        var addressBack = mapper.Map<AddressDto, Address>(addressDto!);

        Assert.NotNull(personDto);
        Assert.NotNull(personBack);
        Assert.NotNull(addressDto);
        Assert.NotNull(addressBack);

        Assert.Equal("Frank", personBack!.FirstName);
        Assert.Equal("Shelbyville", addressBack!.City);
    }

    // -------------------------------------------------------------------------
    // 8. ReverseMap does NOT inherit forward ForMember transforms
    // -------------------------------------------------------------------------

    [Fact]
    public void ReverseMap_ShouldNotInheritForwardForMemberConfiguration()
    {
        var config = new MapperConfiguration(new PersonEmailLowerProfile());
        var mapper = config.CreateMapper();

        // Forward: email is lowercased
        var person = new Person { FirstName = "Gina", LastName = "Hall", Age = 29, Email = "GINA@TEST.COM" };
        var dto = mapper.Map<Person, PersonDto>(person);
        Assert.Equal("gina@test.com", dto!.Email);

        // Reverse: email should NOT be transformed — plain property copy
        var dtoWithMixed = new PersonDto { FirstName = "Gina", LastName = "Hall", Age = 29, Email = "GINA@TEST.COM" };
        var personBack = mapper.Map<PersonDto, Person>(dtoWithMixed);
        Assert.Equal("GINA@TEST.COM", personBack!.Email);
    }

    // -------------------------------------------------------------------------
    // 9. Null source returns null for both directions
    // -------------------------------------------------------------------------

    [Fact]
    public void ReverseMap_NullSource_ForwardDirection_ShouldReturnNull()
    {
        var config = new MapperConfiguration(new PersonReverseProfile());
        var mapper = config.CreateMapper();

        var result = mapper.Map<Person, PersonDto>(null);

        Assert.Null(result);
    }

    [Fact]
    public void ReverseMap_NullSource_ReverseDirection_ShouldReturnNull()
    {
        var config = new MapperConfiguration(new PersonReverseProfile());
        var mapper = config.CreateMapper();

        var result = mapper.Map<PersonDto, Person>(null);

        Assert.Null(result);
    }

    // -------------------------------------------------------------------------
    // 10. Without ReverseMap, reverse direction throws MappingNotFoundException
    // -------------------------------------------------------------------------

    [Fact]
    public void WithoutReverseMap_ReverseDirection_ShouldThrowMappingNotFoundException()
    {
        var config = new MapperConfiguration(new PersonForwardOnlyProfile());
        var mapper = config.CreateMapper();

        var dto = new PersonDto { FirstName = "Hugo", LastName = "Perez", Age = 45 };

        Assert.Throws<MappingNotFoundException>(() => mapper.Map<PersonDto, Person>(dto));
    }

    // -------------------------------------------------------------------------
    // Private test profiles
    // -------------------------------------------------------------------------

    private class PersonReverseProfile : Profile
    {
        public PersonReverseProfile()
        {
            CreateMap<Person, PersonDto>().ReverseMap();
        }
    }

    private class PersonForwardOnlyProfile : Profile
    {
        public PersonForwardOnlyProfile()
        {
            CreateMap<Person, PersonDto>();
        }
    }

    private class PersonEmailLowerProfile : Profile
    {
        public PersonEmailLowerProfile()
        {
            CreateMap<Person, PersonDto>()
                .ForMember(d => d.Email, opt => opt.MapFrom(src => src.Email.ToLower()))
                .ReverseMap();
        }
    }

    private class EmployeeReverseProfile : Profile
    {
        public EmployeeReverseProfile()
        {
            CreateMap<Employee, EmployeeDto>().ReverseMap();
        }
    }

    private class AddressReverseWithForMemberProfile : Profile
    {
        public AddressReverseWithForMemberProfile()
        {
            CreateMap<Address, AddressDto>()
                .ReverseMap()
                .ForMember(d => d.City, opt => opt.MapFrom(src => src.City.ToUpper()));
        }
    }

    private class MultiReverseProfile : Profile
    {
        public MultiReverseProfile()
        {
            CreateMap<Person, PersonDto>().ReverseMap();
            CreateMap<Address, AddressDto>().ReverseMap();
        }
    }
}
