using GameServer.Contracts.Enums;
using GameServer.Contracts.interfaces;
using GameServer.Contracts.PlayerMetaDatas;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Contracts.ServerMessages
{
    public struct PlayerAvatarDataMessage : IServerMessage
    {
        public ServerAction Action => ServerAction.METADATA_PLAYER_INIT;
        public List<PlayaerMetaDataDTO> Players { get; set; }=new List<PlayaerMetaDataDTO>();

        public PlayerAvatarDataMessage() { }
    }
}
