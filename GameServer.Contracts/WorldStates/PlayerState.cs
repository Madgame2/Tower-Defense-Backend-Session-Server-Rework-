using GameServer.Contracts.UDP.Enums;
using GameServer.Domain.Player;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace GameServer.Contracts.WorldStates
{


    public struct PlayerState
    {
        public uint ObjectId;
        public Vector3 Position;
        public Vector3 Velocity;


        public PlayerState(uint objectId, Vector3 position, Vector3 velocity)
        {
            ObjectId = objectId;
            Position = position;
            Velocity = velocity;
        }

        public int Serialize(Span<byte> buffer)
        {
            MemoryMarshal.Write(buffer.Slice(0), ref Unsafe.AsRef(in ObjectId));

            ref byte posBuffer = ref buffer[4];
            MemoryMarshal.Write(MemoryMarshal.CreateSpan(ref posBuffer, 12), ref Unsafe.AsRef(in Position));


            posBuffer = ref buffer[16];
            MemoryMarshal.Write(MemoryMarshal.CreateSpan(ref posBuffer, 12), ref Unsafe.AsRef(in Velocity));

            return 28;
        }
    }
}
