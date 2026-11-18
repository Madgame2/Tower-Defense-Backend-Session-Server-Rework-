using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.RegistryContext.Delegates;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.RegistryContext.Intrerfaces;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.RegistryContext.Base
{
    public sealed class TranslationRule<TIn, TOut> : ITranslationRule
    {
        private readonly Delegate _executor;

        public Type InputType => typeof(TIn);
        public Type OutputType => typeof(TOut);

        public Delegate executor => _executor;

        public TranslationRule(
            Delegate executor)
        {
            _executor = executor;
        }
    }
}