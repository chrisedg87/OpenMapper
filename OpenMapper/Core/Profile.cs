using System.Linq.Expressions;
using OpenMapper.Configuration;

namespace OpenMapper.Core;

public abstract class Profile
{
    internal List<TypeMapConfiguration> TypeMapConfigurations { get; } = new();

    protected IMappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>()
    {
        var config = new TypeMapConfiguration(typeof(TSource), typeof(TDestination));
        TypeMapConfigurations.Add(config);
        return new MappingExpression<TSource, TDestination>(config, TypeMapConfigurations);
    }

    private class MappingExpression<TSource, TDestination> : IMappingExpression<TSource, TDestination>
    {
        private readonly TypeMapConfiguration _config;
        private readonly List<TypeMapConfiguration> _allConfigurations;

        public MappingExpression(TypeMapConfiguration config, List<TypeMapConfiguration> allConfigurations)
        {
            _config = config;
            _allConfigurations = allConfigurations;
        }

        public IMappingExpression<TSource, TDestination> ForMember<TMember>(
            Expression<Func<TDestination, TMember>> destinationMember,
            Action<MemberConfigurationExpression<TSource, TDestination, TMember>> memberOptions)
        {
            var memberName = GetMemberName(destinationMember);
            var memberConfig = new MemberConfiguration<TSource, TDestination>(memberName);
            var configExpression = new MemberConfigurationExpression<TSource, TDestination, TMember>(memberConfig);
            memberOptions(configExpression);
            _config.AddMemberConfiguration(memberName, memberConfig);
            return this;
        }

        public IMappingExpression<TDestination, TSource> ReverseMap()
        {
            var reverseConfig = new TypeMapConfiguration(typeof(TDestination), typeof(TSource));
            _allConfigurations.Add(reverseConfig);
            return new MappingExpression<TDestination, TSource>(reverseConfig, _allConfigurations);
        }

        private static string GetMemberName<TMember>(Expression<Func<TDestination, TMember>> expression)
        {
            if (expression.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }

            throw new ArgumentException("Expression must be a member access expression", nameof(expression));
        }
    }
}
