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
        public bool IsPlaying { get; set; }

        public PlayaerMetaDataDTO()
        {
            Position = new Vector3(1,2,3);
            IsPlaying = true;
        }

        public PlayaerMetaDataDTO(uint playerId,uint objectId, Vector3 position, bool isPlaying)
        {
            PlayerId = playerId;
            ObjectId = objectId;
            Position = position;
            IsPlaying = isPlaying;
        }
    }
}
