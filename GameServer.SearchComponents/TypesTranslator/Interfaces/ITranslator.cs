using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.SearchComponents.TypesTranslator.Interfaces
{
    public interface ITranslator
    {
        Translator Create<TFrom, TTo>();
        Translator Create<TForm>(Type to);
        Translator Create(Type from, Type to);
    }
}
