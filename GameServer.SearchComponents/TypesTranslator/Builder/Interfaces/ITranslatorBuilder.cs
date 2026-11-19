using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.RegistryContext.Intrerfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.SearchComponents.TypesTranslator.Builder.Interfaces
{
    public interface ITranslatorBuilder
    {
        Translator Create<TFrom, TTo>(IObjectRegesterContext regester);
        Translator Create<TForm>(IObjectRegesterContext regester, Type to);
        Translator Create(IObjectRegesterContext regester,Type from, Type to);
    }
}
