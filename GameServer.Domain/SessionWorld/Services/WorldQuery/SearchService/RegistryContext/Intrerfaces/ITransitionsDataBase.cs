using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.RegistryContext.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.RegistryContext.Intrerfaces
{
    public interface ITransitionsDataBase
    {
        void Register(ITranslationRule translationRule);
    }
}
