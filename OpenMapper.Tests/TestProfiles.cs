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

// ForMember test profiles
public class ChapterProfile : Profile
{
    public ChapterProfile()
    {
        CreateMap<Chapter, ChapterDto>()
            .ForMember(m => m.NumberOfLines, opt => opt.MapFrom(src => src.Lines.Count))
            .ForMember(m => m.CoverPhotoUrl, opt => opt.MapFrom(src => src.PhotoUrl))
            .ForMember(m => m.Translation, opt => opt.MapFrom(src => src.Translations.FirstOrDefault()));
    }
}

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(m => m.PriceString, opt => opt.MapFrom(src => src.Price.ToString()))
            .ForMember(m => m.InStock, opt => opt.MapFrom(src => src.Stock > 0));
    }
}

public class PersonWithForMemberProfile : Profile
{
    public PersonWithForMemberProfile()
    {
        CreateMap<Person, PersonDto>()
            .ForMember(m => m.Email, opt => opt.MapFrom(src => src.Email.ToLower()));
        // FirstName, LastName, Age should still auto-map
    }
}
