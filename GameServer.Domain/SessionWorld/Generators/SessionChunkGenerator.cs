using GameServer.Domain.SessionWorld.Graphs.Base.Interfaces;
using GameServer.Domain.SessionWorld.Graphs.BiomGraph.Interfaces;
using GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Base;
using GameServer.Domain.SessionWorld.Meta.Interfaces;
using GameServer.Domain.SessionWorld.Model;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Domain.SessionWorld.Generators
{
    public class SessionChunkGenerator
    {
        private readonly ILandscapeGraphNode _landscapeRoot;
        private readonly IBiomeGraphNode _biomGraphRot;
        private readonly IChunksSettings _chunksSettings;

        public SessionChunkGenerator(ILandscapeGraphNode landscapeRoot, IBiomeGraphNode biomGraphRot, IChunksSettings chunksSettings)
        {
            _landscapeRoot = landscapeRoot;
            _biomGraphRot = biomGraphRot;
            _chunksSettings = chunksSettings;
        }

        public ILandscapeGraphNode LandscapeRoot {  get { return _landscapeRoot; } }
        public IBiomeGraphNode BiomGraphRoot { get { return _biomGraphRot; } }


        public async Task<Chank> CreateChankAsync(Vector2 offset)
        {
            var newChank = new Chank(offset, _chunksSettings.ChunkSize, _chunksSettings.Pivot);

            DefineLandscape(newChank);
            DefineBioms(newChank);

            return newChank;
        }

        private void DefineBioms(Chank chank)
        {
            for (var x = 0; x < chank.Size; x++)
            {
                for (var y = 0; y < chank.Size; y++)
                {
                    var biom = _biomGraphRot.Evaluate(x, y);
                    chank.SetBiomeCell(x, y, biom);
                }
            }
        }

        private void DefineLandscape(Chank chank)
        {
            for (var x = 0; x < chank.Size+1; x++)
            {
                for (var y = 0; y < chank.Size+1; y++)
                {
                    float height = _landscapeRoot.Evaluate(x, y);
                    chank.SetlandscapeHeight(x, y, height);
                }
            }
        }
    }
}
