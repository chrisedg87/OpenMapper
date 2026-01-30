using Microsoft.Extensions.DependencyInjection;
using OpenMapper.Configuration;
using OpenMapper.Core;
using System.Reflection;

namespace OpenMapper.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpenMapper(this IServiceCollection services, params Type[] profileTypes)
    {
        var profiles = profileTypes
            .Select(type => Activator.CreateInstance(type) as Profile)
            .Where(profile => profile != null)
            .Cast<Profile>()
            .ToArray();

        var config = new MapperConfiguration(profiles);
        services.AddSingleton(config);
        services.AddSingleton(sp => sp.GetRequiredService<MapperConfiguration>().CreateMapper());

        return services;
    }

    public static IServiceCollection AddOpenMapper(this IServiceCollection services, params Assembly[] assemblies)
    {
        var profileTypes = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(Profile).IsAssignableFrom(type) && !type.IsAbstract && type.IsClass)
            .ToArray();

        return AddOpenMapper(services, profileTypes);
    }

    public static IServiceCollection AddOpenMapper(this IServiceCollection services, Action<IOpenMapperConfigurationExpression> configAction)
    {
        var configExpression = new OpenMapperConfigurationExpression();
        configAction(configExpression);

        return AddOpenMapper(services, configExpression.ProfileTypes.ToArray());
    }
}

public interface IOpenMapperConfigurationExpression
{
    void AddProfile<TProfile>() where TProfile : Profile;
    void AddProfile(Type profileType);
    void AddProfiles(params Assembly[] assemblies);
}

internal class OpenMapperConfigurationExpression : IOpenMapperConfigurationExpression
{
    public List<Type> ProfileTypes { get; } = new();

    public void AddProfile<TProfile>() where TProfile : Profile
    {
        ProfileTypes.Add(typeof(TProfile));
    }

    public void AddProfile(Type profileType)
    {
        if (!typeof(Profile).IsAssignableFrom(profileType))
            throw new ArgumentException($"Type {profileType.Name} must inherit from Profile", nameof(profileType));

        ProfileTypes.Add(profileType);
    }

    public void AddProfiles(params Assembly[] assemblies)
    {
        var profileTypes = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(Profile).IsAssignableFrom(type) && !type.IsAbstract && type.IsClass);

        ProfileTypes.AddRange(profileTypes);
    }
}
