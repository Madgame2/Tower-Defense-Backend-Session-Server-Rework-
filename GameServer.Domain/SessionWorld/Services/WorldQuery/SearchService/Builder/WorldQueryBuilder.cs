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
        public readonly List<IQueryOperation>Operations = new();

        public WorldQuery<T> Build(out QueryContext context)
        {
            context = new();

            foreach (var operation in this.Operations)
            {
                var parameters = operation.GetParameters();
                context.Link(operation, parameters);
            }

            return new WorldQuery<T>(Operations);
        }
    }
}
