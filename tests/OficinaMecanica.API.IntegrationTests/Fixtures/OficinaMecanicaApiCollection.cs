namespace OficinaMecanica.API.IntegrationTests.Fixtures;

[CollectionDefinition(Nome, DisableParallelization = true)]
public sealed class OficinaMecanicaApiCollection : ICollectionFixture<OficinaMecanicaApiFixture>
{
    public const string Nome = "OficinaMecanicaApi";
}
