using GameServer.Contracts.UDP.Contract;
using GameServer.Contracts.UDP.Enums;
using GameServer.Contracts.WorldStates;
using GameServer.Domain.Player;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace GameServer.Contracts.UDP.Pakets
{
    public ref struct PlayerStateSnapshotWriter
    {
        public static int WriteSnapshot(Span<byte> buffer, uint serverTick, IReadOnlyCollection<Player> players)
        {
            int offset = 0;

            buffer[offset] = (byte)PacketType.PlayerWorldState;
            offset += 1;

            MemoryMarshal.Write(buffer.Slice(offset), ref serverTick);
            offset += 4;

            ushort playersCount = (ushort)players.Count;
            MemoryMarshal.Write(buffer.Slice(offset), ref playersCount);
            offset += 2;

            foreach (var player in players)
            {
                var state = new PlayerState(player.ObjectId, player.Position, player.Velocity, player.MovementState);
                offset += state.Serialize(buffer.Slice(offset));
            }

            return offset;
        }
    }


    public struct PlayerStateSnapshot : IServerUdpPaket
    {
        public PacketType Type => PacketType.PlayerWorldState;
        public uint ServerTick;
        public PlayerState[] PlayersState;


        public PlayerStateSnapshot(uint serverTick)
        {
            ServerTick = serverTick;
        }

        public int Serialize(Span<byte> buffer)
        {
            int offset = 0;

            buffer[offset] = (byte)Type;
            offset += 1;

            uint tick = ServerTick;
            MemoryMarshal.Write(buffer.Slice(offset), ref tick);
            offset += 4;

            ushort playersCount = (ushort)PlayersState.Length;
            MemoryMarshal.Write(buffer.Slice(offset), ref playersCount);
            offset += 2;

            foreach (var playerState in PlayersState)
            {
                offset += playerState.Serialize(buffer.Slice(offset));
            }

            return offset;
        }
    }
}
