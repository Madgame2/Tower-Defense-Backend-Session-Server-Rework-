using GameServer.Domain.SessionWorld;
using GameServer.GameLoop.Core.Simultaion.Steps.Interfaces;
using GameServer.GameObjectComponentsAndServices.GameObjectsComponents.CharacterDefenitions.SpawnPointAnchor;
using GameServer.GameObjectComponentsAndServices.GameObjectsComponents.CharacterDefenitions.UpperBody;
using GameServer.GameObjectComponentsAndServices.GameObjectsComponents.Transform;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.GameLoop.Core.Systems.ResolveSystems
{
    internal class SpawnPointAnchorResolveSystem : ITickable
    {
        public void OnAwake(GameRoom world)
        {
        }

        public Task Tick(float delta, uint tick, GameRoom world)
        {
            foreach (var player in world.AllPlayers)
            {
                if (!player.TryGetComponent<UpperBodyComponent>(
                        out var upperBody))
                    continue;

                if (!player.TryGetComponent<SpawnPointAnchorComponent>(
                        out var anchor))
                    continue;

                if (!player.TryGetComponent<TransformComponent>(
                        out var transform))
                    continue;

                Quaternion rootRotation =
                    Quaternion.Normalize(transform.Rotation);

                Quaternion upperBodyRotation =
                    upperBody.GetWorldRotation(rootRotation);

                Vector3 pivotWorld =
                    transform.Position +
                    Vector3.Transform(
                        anchor.PivotPointLocal,
                        rootRotation);

                Vector3 spawnOffset =
                    anchor.SpawnPointLocal - anchor.PivotPointLocal;

                anchor.WorldSpawnPoint =
                    pivotWorld +
                    Vector3.Transform(
                        spawnOffset,
                        upperBodyRotation);
            }

            return Task.CompletedTask;
        }
    }
}
