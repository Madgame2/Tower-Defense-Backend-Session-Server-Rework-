using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.RegistryContext.Delegates
{
    public delegate bool TranslatorStepDelegate<TIn, TOut>(
        in TIn source,
        out TOut result);
}
