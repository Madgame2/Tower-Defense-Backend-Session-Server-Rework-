using GameServer.SearchComponents.TypesTranslator.Delegates;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.SearchComponents.TypesTranslator
{
    public abstract class Translator
    {
        public Type InputType { get; protected set; }
        public Type OutputType { get; protected set; }

        public abstract Delegate CreateExecutorUntyped();
    }

    public class Translator<TIn, TOut> : Translator
    {
        public Translator()
        {
            InputType = typeof(TIn);
            OutputType = typeof(TOut);
        }

        public TranslatorDelegate<TIn, TOut> CreateExecutor()
        {
            // сворачивание путей в один делегат
            throw new NotImplementedException();
        }

        public override Delegate CreateExecutorUntyped()
        {
            return CreateExecutor();
        }
    }
}
