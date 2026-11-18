using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.SearchComponents.TypesTranslator.API
{
    public interface ITranslationRule
    {
        Type InputType { get; }
        Type OutputType { get; }

        Delegate CreateExecutor();
    }
}
