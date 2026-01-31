using OpenMapper.Configuration;
using OpenMapper.Core;

namespace OpenMapper.Demo;

// Example usage of ForMember functionality

// Source models
public class Book
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public List<string> Chapters { get; set; } = new();
    public decimal Price { get; set; }
    public int Stock { get; set; }
}

// Destination DTO
public class BookDto
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int ChapterCount { get; set; }
    public string PriceDisplay { get; set; } = string.Empty;
    public bool Available { get; set; }
}

// Profile with ForMember configurations
public class BookProfile : Profile
{
    public BookProfile()
    {
        CreateMap<Book, BookDto>()
            // ChapterCount - map from Chapters.Count
            .ForMember(dest => dest.ChapterCount, opt => opt.MapFrom(src => src.Chapters.Count))

            // PriceDisplay - format price as currency string
            .ForMember(dest => dest.PriceDisplay, opt => opt.MapFrom(src => $"${src.Price:F2}"))

            // Available - map from Stock > 0
            .ForMember(dest => dest.Available, opt => opt.MapFrom(src => src.Stock > 0));

        // Title and Author are auto-mapped by convention (matching names)
    }
}

// Demo program
public class Program
{
    public static void Main()
    {
        // Configure the mapper
        var config = new MapperConfiguration(new BookProfile());
        var mapper = config.CreateMapper();

        // Create a source object
        var book = new Book
        {
            Title = "Clean Code",
            Author = "Robert C. Martin",
            Chapters = new List<string>
            {
                "Clean Code",
                "Meaningful Names",
                "Functions",
                "Comments",
                "Formatting"
            },
            Price = 42.99m,
            Stock = 15
        };

        // Map to DTO
        var bookDto = mapper.Map<Book, BookDto>(book);

        // Display results
        Console.WriteLine($"Title: {bookDto.Title}");           // Clean Code (auto-mapped)
        Console.WriteLine($"Author: {bookDto.Author}");         // Robert C. Martin (auto-mapped)
        Console.WriteLine($"Chapters: {bookDto.ChapterCount}"); // 5 (custom mapped from Chapters.Count)
        Console.WriteLine($"Price: {bookDto.PriceDisplay}");    // $42.99 (custom mapped with formatting)
        Console.WriteLine($"Available: {bookDto.Available}");   // True (custom mapped from Stock > 0)
    }
}
