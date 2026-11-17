using EntityLib.Infrastructure;
using System.Numerics;

namespace GameServer.СomponentsForLightweightEntities
{
    public struct RotationComponent : IComponent
    {
        public Quaternion Rotation;
    }
}
