using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.SearchComponents.TypesTranslator.Delegates
{
    public delegate void TranslatorDelegate<TIn, TOut>(in TIn input, out TOut output);
}
