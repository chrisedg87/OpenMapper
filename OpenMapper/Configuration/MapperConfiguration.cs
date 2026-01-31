using OpenMapper.Core;
using OpenMapper.Execution;

namespace OpenMapper.Configuration;

public class MapperConfiguration
{
    private readonly Dictionary<TypePair, TypeMap> _typeMaps = new();

    public MapperConfiguration(params Profile[] profiles)
    {
        foreach (var profile in profiles)
        {
            foreach (var config in profile.TypeMapConfigurations)
            {
                var typePair = new TypePair(config.SourceType, config.DestinationType);
                var typeMap = new TypeMap(config);
                _typeMaps[typePair] = typeMap;
            }
        }
    }

    public IMapper CreateMapper()
    {
        return new Mapper(_typeMaps);
    }
}
