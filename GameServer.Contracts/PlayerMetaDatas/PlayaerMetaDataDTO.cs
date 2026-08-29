using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Contracts.PlayerMetaDatas
{
    public struct PlayaerMetaDataDTO
    {
        public string PlayerId { get; set; }
        public Vector3 Position { get; set; }
        public bool IsPlaying { get; set; }

        public PlayaerMetaDataDTO()
        {
            Position = new Vector3(1,2,3);
            IsPlaying = true;
        }

        public PlayaerMetaDataDTO(string playerId, Vector3 position, bool isPlaying)
        {
            PlayerId = playerId;
            Position = position;
            IsPlaying = isPlaying;
        }
    }
}
