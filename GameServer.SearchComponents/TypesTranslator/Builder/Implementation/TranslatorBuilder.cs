using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.RegistryContext.Intrerfaces;
using GameServer.SearchComponents.TypesTranslator.Builder.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.SearchComponents.TypesTranslator.Builder.Implementation
{
    public class TranslatorBuilder : ITranslatorBuilder
    {
        public Translator Create<TFrom, TTo>(IObjectRegesterContext regester)
        {
            var from = typeof(TFrom);
            var to = typeof(TTo);

            return CreateTranlator(regester, from, to);
        }



        public Translator Create<TFrom>(IObjectRegesterContext regester,Type to)
        {
            var from = typeof(TFrom);


            return CreateTranlator(regester, from, to);
        }

        public Translator Create(IObjectRegesterContext regester,Type from, Type to)
        {
            return CreateTranlator(regester, from, to);
        }

        private static Translator CreateTranlator(IObjectRegesterContext regester, Type from, Type to)
        {
            var transitions = regester.GetWay(from, to);

            var translatorType = typeof(Translator<,>).MakeGenericType(from, to);

            return (Translator)Activator.CreateInstance(translatorType, new object[] { transitions });
        }
    }
}
