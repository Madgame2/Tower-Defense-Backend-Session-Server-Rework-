using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.RegistryContext.DataBase;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.RegistryContext.DataBase.Models;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.RegistryContext.Delegates;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.RegistryContext.Intrerfaces;
using System;
using System.Collections.Generic;
using System.Text;
using static GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.RegistryContext.Base.ObjectRegistryContextBase;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.RegistryContext.Base
{
    public abstract class ObjectRegistryContextBase: IObjectRegesterContext
    {
        private ITransitionsDataBase _dataBase;
        private TransitionRegistrat _registrat;

        public ObjectRegistryContextBase()
        {
            _dataBase = new TransitonsDataBase();
            _registrat = new TransitionRegistrat(_dataBase);
        }

        public abstract void Configure(TransitionRegistrat registrat);

        public TransitionModel[][] GetWay(Type from, Type to)
        {
            return _dataBase.GetTransactions(from, to);
        }

        public class TransitionRegistrat
        {
            private ITransitionsDataBase _dataBase;

            public TransitionRegistrat(ITransitionsDataBase dataBase)
            {
                this._dataBase = dataBase;
            }

            public TransitionRule<TIn, TOut> Register<TIn, TOut>()
            {
                return new TransitionRule<TIn, TOut>(_dataBase);
            }
        }
        public class TransitionRule<TIn, TOut>
        {
            private readonly ITransitionsDataBase _dataBase;

            public TransitionRule(ITransitionsDataBase dataBase)
            {
                _dataBase = dataBase;
            }

            public void Get(TransitionGetDelegate<TIn, TOut> executor)
            {
                _dataBase.Register(
                new TranslationRule<TIn, TOut>(
                    (in TIn source, out TOut result) =>
                        executor(in source, out result)));
            }

            public void Select(Func<TIn, TOut> executor)
            {
                _dataBase.Register(
                    new TranslationRule<TIn, TOut>(
                        (in TIn source, out TOut result) =>
                        {
                            result = executor(source);
                            return true;
                        }));
            }
        }
    }
}
