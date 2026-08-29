using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.Sessions.StateMachine.StatesGraph.Node
{
    public class GraphNode
    {
        public Type Node;
        public List<Type> canTransitTo = new();


        public GraphNode(Type node)
        {
            this.Node = node;
        }
    }
}
