using Microsoft.Extensions.DependencyInjection;
using OpenMapper.Configuration;
using OpenMapper.Core;
using OpenMapper.Extensions;
using System.Reflection;

namespace OpenMapper.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddOpenMapper_WithProfileTypes_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddOpenMapper(typeof(PersonProfile));
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var config = serviceProvider.GetService<MapperConfiguration>();
        var mapper = serviceProvider.GetService<IMapper>();

        Assert.NotNull(config);
        Assert.NotNull(mapper);
    }

    [Fact]
    public void AddOpenMapper_WithMultipleProfileTypes_ShouldRegisterAllProfiles()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddOpenMapper(typeof(PersonProfile), typeof(EmployeeProfile));
        var serviceProvider = services.BuildServiceProvider();
        var mapper = serviceProvider.GetRequiredService<IMapper>();

        // Assert - Test that both profile mappings are available
        var person = new Person { FirstName = "John", LastName = "Doe", Age = 30, Email = "john@example.com" };
        var personDto = mapper.Map<Person, PersonDto>(person);

        var employee = new Employee { FirstName = "Jane", LastName = "Smith", Age = 25 };
        var employeeDto = mapper.Map<Employee, EmployeeDto>(employee);

        Assert.NotNull(personDto);
        Assert.Equal("John", personDto.FirstName);
        Assert.NotNull(employeeDto);
        Assert.Equal("Jane", employeeDto.FirstName);
    }

    [Fact]
    public void AddOpenMapper_WithAssemblies_ShouldScanAndRegisterProfiles()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = Assembly.GetExecutingAssembly();

        // Act
        services.AddOpenMapper(assembly);
        var serviceProvider = services.BuildServiceProvider();
        var mapper = serviceProvider.GetRequiredService<IMapper>();

        // Assert - At least PersonProfile should be found and registered
        var person = new Person { FirstName = "Test", LastName = "User", Age = 30, Email = "test@example.com" };
        var dto = mapper.Map<Person, PersonDto>(person);

        Assert.NotNull(dto);
        Assert.Equal("Test", dto.FirstName);
    }

    [Fact]
    public void AddOpenMapper_WithMultipleAssemblies_ShouldScanAll()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly1 = Assembly.GetExecutingAssembly();
        var assembly2 = typeof(Profile).Assembly; // OpenMapper assembly

        // Act
        services.AddOpenMapper(assembly1, assembly2);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var mapper = serviceProvider.GetService<IMapper>();
        Assert.NotNull(mapper);
    }

    [Fact]
    public void AddOpenMapper_WithConfigurationAction_ShouldRegisterProfiles()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddOpenMapper(cfg =>
        {
            cfg.AddProfile<PersonProfile>();
            cfg.AddProfile<EmployeeProfile>();
        });
        var serviceProvider = services.BuildServiceProvider();
        var mapper = serviceProvider.GetRequiredService<IMapper>();

        // Assert
        var person = new Person { FirstName = "Alice", LastName = "Johnson", Age = 28, Email = "alice@example.com" };
        var dto = mapper.Map<Person, PersonDto>(person);

        Assert.NotNull(dto);
        Assert.Equal("Alice", dto.FirstName);
    }

    [Fact]
    public void AddOpenMapper_WithConfigurationAction_AddProfileByType_ShouldWork()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddOpenMapper(cfg =>
        {
            cfg.AddProfile(typeof(PersonProfile));
        });
        var serviceProvider = services.BuildServiceProvider();
        var mapper = serviceProvider.GetRequiredService<IMapper>();

        // Assert
        var person = new Person { FirstName = "Bob", LastName = "Smith", Age = 35, Email = "bob@example.com" };
        var dto = mapper.Map<Person, PersonDto>(person);

        Assert.NotNull(dto);
        Assert.Equal("Bob", dto.FirstName);
    }

    [Fact]
    public void AddOpenMapper_WithConfigurationAction_AddProfiles_ShouldScanAssemblies()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddOpenMapper(cfg =>
        {
            cfg.AddProfiles(Assembly.GetExecutingAssembly());
        });
        var serviceProvider = services.BuildServiceProvider();
        var mapper = serviceProvider.GetRequiredService<IMapper>();

        // Assert
        Assert.NotNull(mapper);
    }

    [Fact]
    public void AddOpenMapper_MapperConfigurationShouldBeSingleton()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddOpenMapper(typeof(PersonProfile));
        var serviceProvider = services.BuildServiceProvider();

        // Act
        var config1 = serviceProvider.GetRequiredService<MapperConfiguration>();
        var config2 = serviceProvider.GetRequiredService<MapperConfiguration>();

        // Assert
        Assert.Same(config1, config2); // Should be the same instance
    }

    [Fact]
    public void AddOpenMapper_MapperShouldBeSingleton()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddOpenMapper(typeof(PersonProfile));
        var serviceProvider = services.BuildServiceProvider();

        // Act
        var mapper1 = serviceProvider.GetRequiredService<IMapper>();
        var mapper2 = serviceProvider.GetRequiredService<IMapper>();

        // Assert
        Assert.Same(mapper1, mapper2); // Should be the same instance
    }

    [Fact]
    public void AddOpenMapper_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddOpenMapper(typeof(PersonProfile));

        // Assert
        Assert.Same(services, result); // Should return the same collection for chaining
    }

    [Fact]
    public void AddOpenMapper_WithInvalidProfileType_ShouldThrowArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
        {
            services.AddOpenMapper(cfg =>
            {
                cfg.AddProfile(typeof(Person)); // Not a Profile type
            });
        });

        Assert.Contains("Profile", exception.Message);
    }

    [Fact]
    public void AddOpenMapper_WithNoProfiles_ShouldStillRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddOpenMapper(Array.Empty<Type>());
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var mapper = serviceProvider.GetService<IMapper>();
        var config = serviceProvider.GetService<MapperConfiguration>();

        Assert.NotNull(mapper);
        Assert.NotNull(config);
    }

    [Fact]
    public void AddOpenMapper_WithEmptyProfile_ShouldNotThrow()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddOpenMapper(typeof(EmptyProfile));
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var mapper = serviceProvider.GetService<IMapper>();
        Assert.NotNull(mapper);
    }

    [Fact]
    public void AddOpenMapper_IntegrationTest_ShouldWorkInRealScenario()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddOpenMapper(cfg =>
        {
            cfg.AddProfile<PersonProfile>();
            cfg.AddProfile<AddressProfile>();
        });
        var serviceProvider = services.BuildServiceProvider();

        // Act
        var mapper = serviceProvider.GetRequiredService<IMapper>();
        var person = new Person
        {
            FirstName = "Integration",
            LastName = "Test",
            Age = 40,
            Email = "integration@test.com"
        };
        var dto = mapper.Map<Person, PersonDto>(person);

        // Also test address mapping
        var address = new Address
        {
            Street = "456 Test Ave",
            City = "Test City",
            ZipCode = "99999"
        };
        var addressDto = mapper.Map<Address, AddressDto>(address);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal("Integration", dto.FirstName);
        Assert.Equal("Test", dto.LastName);
        Assert.Equal(40, dto.Age);
        Assert.Equal("integration@test.com", dto.Email);

        Assert.NotNull(addressDto);
        Assert.Equal("456 Test Ave", addressDto.Street);
        Assert.Equal("Test City", addressDto.City);
        Assert.Equal("99999", addressDto.ZipCode);
    }
}
