using GameServer.Domain.SessionWorld.Graphs.Base.Meta;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Graphs.Base.Interfaces
{
    public interface ICondition
    {
        bool Pass(float x, float y);
        IGraphNode[] GetConditionNodes(); 
        ConditionData GetMetaData(Func<IGraphNode, short> serializeCallback);
    }
}
