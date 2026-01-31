# OpenMapper

A lightweight object-to-object mapping library for .NET 8 with convention-based property mapping and a simple, AutoMapper-inspired API.

[![NuGet](https://img.shields.io/nuget/v/OpenMapper.svg)](https://www.nuget.org/packages/OpenMapper/)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)

## Features

- **Convention-Based Mapping**: Automatically maps properties with matching names and compatible types
- **Custom Member Mapping**: Use `ForMember` to define custom mapping expressions for any property
- **Profile Organization**: Group related mappings into reusable Profile classes
- **Dependency Injection Ready**: First-class support for .NET DI container with multiple registration styles
- **Simple API**: Clean, intuitive interface inspired by AutoMapper
- **Zero Configuration for Simple Cases**: Just create a map and go
- **Lightweight**: Minimal dependencies and straightforward implementation
- **Type-Safe**: Full generic type support with compile-time type checking

## Installation

Install via NuGet Package Manager:

```bash
dotnet add package OpenMapper
```

Or via Package Manager Console:

```powershell
Install-Package OpenMapper
```

## Quick Start

### Basic Usage

```csharp
using OpenMapper;
using OpenMapper.Configuration;
using OpenMapper.Core;

// Define your models
public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

public class UserViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

// Create a profile
public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserDto, UserViewModel>();
    }
}

// Configure and use the mapper
var config = new MapperConfiguration(cfg =>
{
    cfg.AddProfile<UserProfile>();
});

var mapper = config.CreateMapper();

var userDto = new UserDto
{
    Id = 1,
    Name = "John Doe",
    Email = "john@example.com"
};

var viewModel = mapper.Map<UserViewModel>(userDto);
```

### Dependency Injection Integration

OpenMapper provides three convenient ways to register with the .NET DI container:

#### Option 1: Pass Profile Types Directly

```csharp
using OpenMapper.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenMapper(typeof(UserProfile), typeof(OrderProfile));
```

#### Option 2: Auto-Scan Assemblies

```csharp
using OpenMapper.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Scan the executing assembly for all Profile classes
builder.Services.AddOpenMapper(Assembly.GetExecutingAssembly());
```

#### Option 3: Fluent Configuration

```csharp
using OpenMapper.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenMapper(cfg =>
{
    cfg.AddProfile<UserProfile>();
    cfg.AddProfile<OrderProfile>();
});
```

Then inject `IMapper` into your services:

```csharp
public class UserService
{
    private readonly IMapper _mapper;

    public UserService(IMapper mapper)
    {
        _mapper = mapper;
    }

    public UserViewModel GetUser(int id)
    {
        var userDto = GetUserFromDatabase(id);
        return _mapper.Map<UserViewModel>(userDto);
    }
}
```

## Detailed Usage

### Creating Profiles

Profiles help organize related mappings:

```csharp
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Map from source to destination
        CreateMap<Customer, CustomerDto>();
        CreateMap<Order, OrderDto>();
        CreateMap<Product, ProductDto>();

        // Create reverse mappings if needed
        CreateMap<CustomerDto, Customer>();
        CreateMap<OrderDto, Order>();
    }
}
```

### Mapping Methods

OpenMapper provides three mapping methods through the `IMapper` interface:

#### 1. Generic Mapping (Recommended)

```csharp
var destination = mapper.Map<TDestination>(source);

// Example
var viewModel = mapper.Map<UserViewModel>(userDto);
```

#### 2. Non-Generic Mapping

```csharp
object destination = mapper.Map(source, sourceType, destinationType);

// Example
var viewModel = mapper.Map(userDto, typeof(UserDto), typeof(UserViewModel));
```

#### 3. Map to Existing Instance

```csharp
mapper.Map(source, existingDestination);

// Example - useful for updating existing objects
var existingUser = new UserViewModel { Id = 1, Name = "Old Name" };
mapper.Map(userDto, existingUser);
// existingUser is now updated with values from userDto
```

### Convention-Based Mapping Rules

OpenMapper automatically maps properties when:

1. **Names match**: Source and destination properties have the same name (case-sensitive)
2. **Types are compatible**: Destination type is assignable from source type

```csharp
public class Source
{
    public int Id { get; set; }           // ✓ Maps to Id
    public string Name { get; set; }      // ✓ Maps to Name
    public DateTime Created { get; set; } // ✓ Maps to Created
    public int Age { get; set; }          // ✗ No matching property in destination
}

public class Destination
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime Created { get; set; }
    public string Email { get; set; }     // ✗ No matching property in source (will be default)
}
```

### Custom Member Mapping with ForMember

Use `ForMember` to customize how individual properties are mapped. This is useful when:
- Property names don't match
- You need to transform or calculate values
- You need to map from nested properties
- Types need conversion

```csharp
public class Book
{
    public string Title { get; set; }
    public List<string> Chapters { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
}

public class BookDto
{
    public string Title { get; set; }
    public int ChapterCount { get; set; }
    public string PriceDisplay { get; set; }
    public bool Available { get; set; }
}

public class BookProfile : Profile
{
    public BookProfile()
    {
        CreateMap<Book, BookDto>()
            // Map from a collection count
            .ForMember(dest => dest.ChapterCount,
                opt => opt.MapFrom(src => src.Chapters.Count))

            // Format a value with custom logic
            .ForMember(dest => dest.PriceDisplay,
                opt => opt.MapFrom(src => $"${src.Price:F2}"))

            // Map with a boolean expression
            .ForMember(dest => dest.Available,
                opt => opt.MapFrom(src => src.Stock > 0));

        // Title is auto-mapped by convention
    }
}
```

#### Complex Expressions

`ForMember` supports any valid C# expression:

```csharp
public class UserDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Address Address { get; set; }
    public List<Order> Orders { get; set; }
}

public class UserViewModel
{
    public string FullName { get; set; }
    public string City { get; set; }
    public int OrderCount { get; set; }
    public decimal TotalSpent { get; set; }
}

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserDto, UserViewModel>()
            // String concatenation
            .ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))

            // Nested property access
            .ForMember(dest => dest.City,
                opt => opt.MapFrom(src => src.Address.City))

            // LINQ expressions
            .ForMember(dest => dest.OrderCount,
                opt => opt.MapFrom(src => src.Orders.Count))

            .ForMember(dest => dest.TotalSpent,
                opt => opt.MapFrom(src => src.Orders.Sum(o => o.Total)));
    }
}
```

#### Type Conversion

`ForMember` automatically converts between compatible types:

```csharp
public class Product
{
    public int Id { get; set; }
    public decimal Price { get; set; }
}

public class ProductDto
{
    public string Id { get; set; }        // int → string conversion
    public string Price { get; set; }     // decimal → string conversion
}

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Id))        // Converts int to string

            .ForMember(dest => dest.Price,
                opt => opt.MapFrom(src => src.Price));    // Converts decimal to string
    }
}
```

#### Handling Null Values

When mapping from properties that might be null, handle nulls in your expression:

```csharp
public class PersonDto
{
    public Address Address { get; set; }  // Might be null
}

public class PersonViewModel
{
    public string City { get; set; }
}

public class PersonProfile : Profile
{
    public PersonProfile()
    {
        CreateMap<PersonDto, PersonViewModel>()
            // Safe navigation with null-conditional operator
            .ForMember(dest => dest.City,
                opt => opt.MapFrom(src => src.Address?.City ?? "Unknown"))

            // Or use null-coalescing
            .ForMember(dest => dest.City,
                opt => opt.MapFrom(src => src.Address != null ? src.Address.City : "N/A"));
    }
}
```

### Working with Complex Scenarios

#### Mapping Collections

```csharp
var users = new List<UserDto>
{
    new UserDto { Id = 1, Name = "John" },
    new UserDto { Id = 2, Name = "Jane" }
};

var viewModels = users.Select(u => mapper.Map<UserViewModel>(u)).ToList();
```

#### Nested Objects

```csharp
public class OrderDto
{
    public int Id { get; set; }
    public CustomerDto Customer { get; set; }
    public List<OrderItemDto> Items { get; set; }
}

public class OrderViewModel
{
    public int Id { get; set; }
    public CustomerViewModel Customer { get; set; }
    public List<OrderItemViewModel> Items { get; set; }
}

// Create mappings for all types
public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<OrderDto, OrderViewModel>();
        CreateMap<CustomerDto, CustomerViewModel>();
        CreateMap<OrderItemDto, OrderItemViewModel>();
    }
}

// Map the entire object graph
var orderViewModel = mapper.Map<OrderViewModel>(orderDto);
// Note: Nested objects are NOT automatically mapped - you need to map them separately
```

## API Reference

### `MapperConfiguration`

Entry point for configuring mappings.

```csharp
public MapperConfiguration(Action<MapperConfiguration> configure)
public void AddProfile<TProfile>() where TProfile : Profile, new()
public void AddProfile(Profile profile)
public IMapper CreateMapper()
```

### `Profile`

Base class for organizing mappings.

```csharp
public abstract class Profile
{
    protected IMappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>();
}
```

### `IMappingExpression<TSource, TDestination>`

Fluent interface for configuring type mappings.

```csharp
public interface IMappingExpression<TSource, TDestination>
{
    IMappingExpression<TSource, TDestination> ForMember<TMember>(
        Expression<Func<TDestination, TMember>> destinationMember,
        Action<MemberConfigurationExpression<TSource, TDestination, TMember>> memberOptions);
}
```

### `MemberConfigurationExpression<TSource, TDestination, TMember>`

Configures custom mapping for a specific member.

```csharp
public class MemberConfigurationExpression<TSource, TDestination, TMember>
{
    public void MapFrom<TSourceMember>(Expression<Func<TSource, TSourceMember>> sourceMember);
}
```

### `IMapper`

Main interface for performing mappings.

```csharp
public interface IMapper
{
    TDestination Map<TDestination>(object source);
    object Map(object source, Type sourceType, Type destinationType);
    void Map(object source, object destination);
}
```

## Requirements

- .NET 8.0 or later
- Microsoft.Extensions.DependencyInjection.Abstractions 8.0.0 (automatically installed)

## Limitations

Current version limitations (future enhancements planned):

- No value converters or type converters (basic type conversion supported via `ForMember`)
- No conditional mapping
- No before/after map actions
- No `Ignore()` support for explicitly ignoring properties
- Destination types must have parameterless constructors
- No automatic nested object mapping (you must map nested objects explicitly or use `ForMember`)

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Building from Source

```bash
# Clone the repository
git clone https://github.com/chrisedg87/OpenMapper.git
cd OpenMapper

# Build the project
dotnet build OpenMapper.sln

# Run tests
dotnet test OpenMapper.Tests/OpenMapper.Tests.csproj

# Create NuGet package
dotnet pack OpenMapper.csproj -c Release
```

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

Inspired by [AutoMapper](https://automapper.org/), OpenMapper provides a simpler, more lightweight alternative for projects that need basic object mapping functionality without the complexity of larger frameworks.

## Support

- Report issues: [GitHub Issues](https://github.com/chrisedg87/OpenMapper/issues)
- NuGet Package: [OpenMapper](https://www.nuget.org/packages/OpenMapper/)