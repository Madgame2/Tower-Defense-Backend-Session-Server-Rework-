using EntityLib.Infrastructure;

namespace GameServer.СomponentsForLightweightEntities
{
    public struct IDComponent : IComponent
    {
        public uint ObjectId { get; }

        public IDComponent()
        {
            Span<byte> buffer = stackalloc byte[4];
            Random.Shared.NextBytes(buffer);

            ObjectId = BitConverter.ToUInt32(buffer);
        }
    }
}
