using OpenMapper.Core;

namespace OpenMapper.Tests;

public class PersonProfile : Profile
{
    public PersonProfile()
    {
        CreateMap<Person, PersonDto>();
        CreateMap<PersonDto, Person>();
    }
}

public class EmployeeProfile : Profile
{
    public EmployeeProfile()
    {
        CreateMap<Employee, EmployeeDto>();
    }
}

public class AddressProfile : Profile
{
    public AddressProfile()
    {
        CreateMap<Address, AddressDto>();
        CreateMap<AddressDto, Address>();
    }
}

public class NestedProfile : Profile
{
    public NestedProfile()
    {
        CreateMap<PersonWithAddress, PersonWithAddressDto>();
        CreateMap<Address, AddressDto>();
    }
}

public class EmptyProfile : Profile
{
    public EmptyProfile()
    {
        // No mappings defined
    }
}

public class PartialMappingProfile : Profile
{
    public PartialMappingProfile()
    {
        CreateMap<Person, PartialPersonDto>();
    }
}
