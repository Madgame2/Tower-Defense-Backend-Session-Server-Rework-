using GameServer.Contracts.WorldMetadata;
using GameServer.Domain.SessionWorld.Graphs.Base.Interfaces;
using GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Nodes.MetaData;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Infrastructure.Serialization
{
    public class GraphSerializer
    {
        private readonly Dictionary<IGraphNode, short> _visitedNodes = new();
        private readonly List<GraphNodeDTO> _serializedNodes = new();

        public GraphNodeDTO[] SerializeGraph(IGraphNode rootNode)
        {
            _serializedNodes.Clear();
            _visitedNodes.Clear();

            SerializeRecursive(rootNode, -1);

            return _serializedNodes.ToArray();
        }


        private short SerializeRecursive(IGraphNode node, short parentIndex)
        {
            if (node == null) return -1;

            if (_visitedNodes.TryGetValue(node, out short existingIndex))
            {
                return existingIndex;
            }

            short currentIndex = (short)_serializedNodes.Count;
            _serializedNodes.Add(default);
            _visitedNodes[node] = currentIndex;

            IGraphNode[] children = node.GetChildren();
            short[] childIndices = new short[children.Length];

            for (int i = 0; i < children.Length; i++)
            {
                childIndices[i] = SerializeRecursive(children[i], currentIndex);
            }

            NodeParam[] parameters = node.GetParams(n => SerializeRecursive(n, -1));

            _serializedNodes[currentIndex] = new GraphNodeDTO(
                node.Type,
                parentIndex,
                childIndices,
                parameters
            );

            return currentIndex;
        }
    }
}
