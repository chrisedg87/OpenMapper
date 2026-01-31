using System.Linq.Expressions;

namespace OpenMapper.Configuration;

public interface IMappingExpression<TSource, TDestination>
{
    IMappingExpression<TSource, TDestination> ForMember<TMember>(
        Expression<Func<TDestination, TMember>> destinationMember,
        Action<MemberConfigurationExpression<TSource, TDestination, TMember>> memberOptions);
}
