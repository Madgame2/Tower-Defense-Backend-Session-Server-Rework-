using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.SearchComponents.TypesTranslator.Delegates
{
    public delegate bool TransitionDelegate<TIn, TOut>(
        in TIn source,
        out TOut result);
}
