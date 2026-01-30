namespace OpenMapper.Tests;

// Source models
public class Person
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Email { get; set; } = string.Empty;
}

public class Employee
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Department { get; set; } = string.Empty;
    public decimal Salary { get; set; }
}

public class Address
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
}

// Destination models
public class PersonDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Email { get; set; } = string.Empty;
}

public class EmployeeDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int Age { get; set; }
}

public class AddressDto
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
}

// Model with incompatible types
public class IncompatibleModel
{
    public int FirstName { get; set; } // Incompatible type
    public string LastName { get; set; } = string.Empty;
}

// Model with read-only properties
public class ReadOnlyModel
{
    public string FirstName { get; }
    public string LastName { get; set; } = string.Empty;

    public ReadOnlyModel(string firstName)
    {
        FirstName = firstName;
    }
}

// Model with write-only source property
public class WriteOnlySourceModel
{
    private string _name = string.Empty;
    public string Name
    {
        set => _name = value;
    }
}

// Model with different property subset
public class PartialPersonDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

// Nested models
public class PersonWithAddress
{
    public string Name { get; set; } = string.Empty;
    public Address? Address { get; set; }
}

public class PersonWithAddressDto
{
    public string Name { get; set; } = string.Empty;
    public AddressDto? Address { get; set; }
}
