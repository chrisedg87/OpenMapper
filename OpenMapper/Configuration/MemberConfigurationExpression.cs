using System.Linq.Expressions;

namespace OpenMapper.Configuration;

public class MemberConfigurationExpression<TSource, TDestination, TMember>
{
    private readonly MemberConfiguration<TSource, TDestination> _memberConfiguration;

    internal MemberConfigurationExpression(MemberConfiguration<TSource, TDestination> memberConfiguration)
    {
        _memberConfiguration = memberConfiguration;
    }

    public void MapFrom<TSourceMember>(Expression<Func<TSource, TSourceMember>> sourceMember)
    {
        var compiled = sourceMember.Compile();
        _memberConfiguration.ValueResolver = source => compiled(source);
    }
}
