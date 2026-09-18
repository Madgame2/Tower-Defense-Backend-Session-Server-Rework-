using GameServer.Contracts.Enums;
using GameServer.Contracts.interfaces;
using GameServer.Contracts.WorldMetadata;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Contracts.ServerMessages
{
    [MessagePackObject]
    public struct WorldGenerationMetaDataMessage : IServerMessage
    {
        [Key(0)]
        public ServerAction Action => ServerAction.METADATA_WORLD_GENERATION_SETTINGS;

        [Key(1)]
        public GraphNodeDTO[] LandscapeGraphDTOs { get; set; }
        [Key(2)]
        public GraphNodeDTO[] BiomsGraphDTOs { get; set; }

        [Key(3)]
        public GraphNodeDTO[] TreeGraphDTOs { get; set; }

        public WorldGenerationMetaDataMessage(GraphNodeDTO[] landscapeGraphDTOs, GraphNodeDTO[] biomsGraphDTOs, GraphNodeDTO[] treeGraphDTOs)
        {
            LandscapeGraphDTOs = landscapeGraphDTOs;
            BiomsGraphDTOs = biomsGraphDTOs;
            TreeGraphDTOs = treeGraphDTOs;
        }
    }
}
