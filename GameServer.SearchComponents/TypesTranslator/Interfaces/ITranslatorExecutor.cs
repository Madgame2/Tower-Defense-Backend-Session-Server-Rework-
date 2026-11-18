using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.SearchComponents.TypesTranslator.Interfaces
{
    public interface ITranslatorExecutor<TIn, TOut>
    {
        void Execute(in TIn input, out TOut output);

    }
}
