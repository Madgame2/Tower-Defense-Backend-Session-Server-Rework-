using GameServer.Domain.Core.GameObjects.Components;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.GameObjectComponentsAndServices.GameObjectsComponents.CharacterDefenitions.SpawnPointAnchor
{
    public class SpawnPointAnchorComponent : ComponentBase
    {
        public string AnchorName { get; set; }

        // Исходная локальная позиция точки спавна.
        public Vector3 SpawnPointLocal;

        // Исходная локальная позиция Pivot.
        public Vector3 PivotPointLocal;

        // Рассчитанная мировая позиция точки спавна.
        public Vector3 WorldSpawnPoint;

        public override ComponentBase Clone()
        {
            return new SpawnPointAnchorComponent
            {
                AnchorName = AnchorName,
                SpawnPointLocal = SpawnPointLocal,
                PivotPointLocal = PivotPointLocal,
                WorldSpawnPoint = WorldSpawnPoint
            };
        }
    }
}
