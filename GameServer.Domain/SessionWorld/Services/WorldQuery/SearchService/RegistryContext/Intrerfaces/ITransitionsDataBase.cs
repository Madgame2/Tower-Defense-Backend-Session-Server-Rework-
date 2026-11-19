using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.RegistryContext.Base;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.RegistryContext.DataBase.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.RegistryContext.Intrerfaces
{
    public interface ITransitionsDataBase
    {
        TransitionModel[][] GetTransactions(Type from, Type to);
        void Register(ITranslationRule translationRule);
    }
}
