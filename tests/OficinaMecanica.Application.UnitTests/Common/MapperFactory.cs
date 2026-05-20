using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using OficinaMecanica.Application;

namespace OficinaMecanica.Application.UnitTests.Common;

internal static class MapperFactory
{
    public static IMapper Criar()
    {
        var configuration = new MapperConfiguration(config => config.AddMaps(typeof(DependencyInjection).Assembly), NullLoggerFactory.Instance);

        configuration.AssertConfigurationIsValid();

        return configuration.CreateMapper();
    }
}
