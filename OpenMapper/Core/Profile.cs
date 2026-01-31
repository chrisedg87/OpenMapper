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
        return new MappingExpression<TSource, TDestination>(config);
    }

    private class MappingExpression<TSource, TDestination> : IMappingExpression<TSource, TDestination>
    {
        private readonly TypeMapConfiguration _config;

        public MappingExpression(TypeMapConfiguration config)
        {
            _config = config;
        }

        public IMappingExpression<TSource, TDestination> ForMember<TMember>(
            Expression<Func<TDestination, TMember>> destinationMember,
            Action<MemberConfigurationExpression<TSource, TDestination, TMember>> memberOptions)
        {
            // Extract property name from expression
            var memberName = GetMemberName(destinationMember);

            // Create member configuration
            var memberConfig = new MemberConfiguration<TSource, TDestination>(memberName);

            // Create expression and let user configure it
            var configExpression = new MemberConfigurationExpression<TSource, TDestination, TMember>(memberConfig);
            memberOptions(configExpression);

            // Store in configuration
            _config.AddMemberConfiguration(memberName, memberConfig);

            return this;
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
