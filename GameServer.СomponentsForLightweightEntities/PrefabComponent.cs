using EntityLib.Infrastructure;
using GameServer.Domain.Core.GameObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.СomponentsForLightweightEntities
{
    public struct PrefabComponent : IComponent
    {
        public GameObject Prefab;
    }
}
