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
        private QueryContext<StaticTreeData> queryContext;

        private List<StaticTreeData> buffer = new(32);
        public void OnAwake(GameRoom world)
        {

            _treeQuery = world.worldQueryService
                .Search<StaticTreeData>()
                .InRadius("PlayerPos",1f)
                .Build(out var query1Context);

            queryContext = query1Context;
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
                    var treePosition = new Vector3(entity.LocalPosition.X, 0, entity.LocalPosition.Y);

                    // Считаем расстояние до игрока
                    float distance = Vector3.Distance(treePosition, player.Position);

                    // Минимальное расстояние, на которое нельзя подходить (радиус коллизии)
                    float minDistance = 1.0f;

                    if (distance < minDistance && distance > 0.0001f) // Проверяем, что не в одной точке
                    {
                        // 1. Находим направление ОТ дерева К игроку
                        var pushDir = Vector3.Normalize(player.Position - treePosition);

                        // 2. Вычисляем, на какое расстояние нужно вытолкать
                        float penetrationDepth = minDistance - distance;

                        // 3. Отодвигаем игрока на это расстояние наружу
                        player.Position += pushDir * penetrationDepth;
                    }
                }
            }
        }
    }
}
