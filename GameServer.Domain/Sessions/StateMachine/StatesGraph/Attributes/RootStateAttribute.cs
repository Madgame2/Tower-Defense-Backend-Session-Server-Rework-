using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.Sessions.StateMachine.StatesGraph.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RootStateAttribute : Attribute
    {
    }
}
