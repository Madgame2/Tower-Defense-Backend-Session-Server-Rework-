using GameServer.Domain.SessionWorld.Graphs.Base.Interfaces;
using GameServer.Domain.SessionWorld.Graphs.BiomGraph.Interfaces;
using GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Base;
using GameServer.Domain.SessionWorld.Graphs.TreeGraph;
using GameServer.Domain.SessionWorld.Meta.Interfaces;
using GameServer.Domain.SessionWorld.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace GameServer.Domain.SessionWorld.Generators
{
    public class SessionChunkGenerator
    {
        private readonly ILandscapeGraphNode _landscapeRoot;
        private readonly IBiomeGraphNode _biomGraphRot;
        private readonly IGraphNode<StaticTreeData> _treeGraphRoot;
        private readonly IChunksSettings _chunksSettings;

        public SessionChunkGenerator(ILandscapeGraphNode landscapeRoot, IBiomeGraphNode biomGraphRot, IGraphNode<StaticTreeData> treeRoot, IChunksSettings chunksSettings)
        {
            _landscapeRoot = landscapeRoot;
            _biomGraphRot = biomGraphRot;
            _treeGraphRoot = treeRoot;
            _chunksSettings = chunksSettings;
        }

        public ILandscapeGraphNode LandscapeRoot {  get { return _landscapeRoot; } }
        public IBiomeGraphNode BiomGraphRoot { get { return _biomGraphRot; } }
        public IGraphNode<StaticTreeData> TreeGraphRoot {  get { return _treeGraphRoot; } }

        public async Task<Chank> CreateChankAsync(Vector2 offset)
        {
            var newChank = new Chank(offset, _chunksSettings.ChunkSize, _chunksSettings.Pivot);

            DefineLandscape(newChank);
            DefineBioms(newChank);
            PlaceTrees(newChank);

            return newChank;
        }

        private void PlaceTrees(Chank chank)
        {
            int baseSize = _chunksSettings.ChunkSize;
            float chunkWorldOriginX = chank.Position.X * baseSize;
            float chunkWorldOriginY = chank.Position.Y * baseSize;

            Vector2 pivotOffset = chank.Pivot * baseSize;

            for (var x = 0; x < chank.Size; x++)
            {
                for (var y = 0; y < chank.Size; y++)
                {
                    float worldX = chunkWorldOriginX + x - pivotOffset.X;
                    float worldY = chunkWorldOriginY + y - pivotOffset.Y;

                    try
                    {
                        var hasTree = TreeGraphRoot.TryEvaluate(worldX, worldY, out var data);
                        if (hasTree)
                            chank.AddTree(data);
                    }
                    catch (Exception exception)
                    {
                        
                    }

                }
            }
        }

        private void DefineBioms(Chank chank)
        {
            int baseSize = _chunksSettings.ChunkSize;
            float chunkWorldOriginX = chank.Position.X * baseSize;
            float chunkWorldOriginY = chank.Position.Y * baseSize;

            Vector2 pivotOffset = chank.Pivot * baseSize;

            for (var x = 0; x < chank.Size; x++)
            {
                for (var y = 0; y < chank.Size; y++)
                {
                    float worldX = chunkWorldOriginX + x - pivotOffset.X;
                    float worldY = chunkWorldOriginY + y - pivotOffset.Y;

                    var biom = _biomGraphRot.Evaluate(worldX, worldY);
                    chank.SetBiomeCell(x, y, biom);
                }
            }
        }

        private void DefineLandscape(Chank chank)
        {
            int baseSize = _chunksSettings.ChunkSize;
            float chunkWorldOriginX = chank.Position.X * baseSize;
            float chunkWorldOriginY = chank.Position.Y * baseSize;

            Vector2 pivotOffset = chank.Pivot * baseSize;

            for (var x = 0; x < chank.Size+1; x++)
            {
                for (var y = 0; y < chank.Size+1; y++)
                {
                    float worldX = chunkWorldOriginX + x - pivotOffset.X;
                    float worldY = chunkWorldOriginY + y - pivotOffset.Y;

                    _landscapeRoot.TryEvaluate(worldX, worldY, out var height);
                    chank.SetlandscapeHeight(x, y, height);
                }
            }
        }
    }
}
