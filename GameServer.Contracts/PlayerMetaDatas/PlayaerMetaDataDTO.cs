using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Contracts.PlayerMetaDatas
{
    public struct PlayaerMetaDataDTO
    {
        public uint PlayerId { get; set; }
        public uint ObjectId { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public bool IsPlaying { get; set; }

        public PlayaerMetaDataDTO(uint playerId,uint objectId, Quaternion rotation, Vector3 position, bool isPlaying)
        {
            PlayerId = playerId;
            ObjectId = objectId;
            Rotation = rotation;
            Position = position;
            IsPlaying = isPlaying;
        }
    }
}
