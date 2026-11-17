using GameServer.Domain.Core.GameObjects.Components;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.GameObjectComponentsAndServices.GameObjectsComponents.Phisycs
{
    public class VelocityComponent :ComponentBase
    {
        public Vector3 Velocity;

        public override ComponentBase Clone()
        {
            return new VelocityComponent
            {
                Velocity = Velocity
            };
        }
    }
}
