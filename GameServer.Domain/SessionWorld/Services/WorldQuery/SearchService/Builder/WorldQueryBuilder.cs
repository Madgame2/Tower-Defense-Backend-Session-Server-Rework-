using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Interfaces;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model.Parameters;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model.Parameters.Base;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators.Interfaces;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Builder
{
    public class WorldQueryBuilder
    {
        public static AttachedParameter<T> Parameter<T>()
        {
            return new AttachedParameter<T>();
        }
    }

    public sealed class WorldQueryBuilder<T>
    {
        private GameRoom _gameRoom;
        private readonly IQueryPlanner _queryPlanner;


        public WorldQueryBuilder(GameRoom gameRoom, IQueryPlanner queryPlanner)
        {
            _gameRoom = gameRoom;
            _queryPlanner = queryPlanner;
        }

        public readonly List<IQueryOperation>Operations = new();

        public QueryExecutePlan<T> Build(out QueryContext context)
        {
            context = new(_gameRoom.IndexStorage);

            foreach (var operation in this.Operations)
            {
                var parameters = operation.GetParameters();
                context.Link(operation, parameters);
            }

            var worldQuery = new WorldQuery<T>(Operations);

            return _queryPlanner.Build(worldQuery, context);
        }
    }
}
