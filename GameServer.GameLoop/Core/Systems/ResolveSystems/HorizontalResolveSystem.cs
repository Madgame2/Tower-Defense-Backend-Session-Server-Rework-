using GameServer.Domain.SessionWorld;
using GameServer.Domain.SessionWorld.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Builder;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators.Extentions;
using GameServer.GameLoop.Core.Simultaion.Steps.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Numerics;
using System.Text;

namespace GameServer.GameLoop.Core.Systems.ResolveSystems
{
    internal class HorizontalResolveSystem : ITickable
    {
        private QueryExecutePlan<StaticTreeData> _treeQuery;
        private QueryContext queryContext;

        private List<StaticTreeData> buffer = new(32);
        public void OnAwake(GameRoom world)
        {

            _treeQuery = world.worldQueryService
                .Search<StaticTreeData>()
                .InRadius("PlayerPos",1f)
                .Build(out var query1Context);


            var parametr = WorldQueryBuilder.Parameter<Vector3>();
            _treeQuery = world.worldQueryService
                .Search<StaticTreeData>()
                .InRadius(parametr, 1f)
                .Build(out var query2Context);

            queryContext = query2Context;
        }

        public async Task Tick(float delta, GameRoom world)
        {
            var players = world.AllPlayers;

            foreach (var player in players)
            {
                queryContext.Set("PlayerPos", player.Position);
                world.worldQueryService.ExecuteQuery(_treeQuery, queryContext, buffer);

                foreach (var entity in buffer)
                {

                }
            }
        }
    }
}
