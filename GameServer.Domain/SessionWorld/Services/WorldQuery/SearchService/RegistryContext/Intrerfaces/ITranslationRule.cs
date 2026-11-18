namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.RegistryContext.Intrerfaces
{
    public interface ITranslationRule
    {
        Type InputType { get; }
        Type OutputType { get; }

        Delegate executor { get; }
    }
}