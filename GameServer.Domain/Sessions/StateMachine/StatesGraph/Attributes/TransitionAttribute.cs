using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.Sessions.StateMachine.StatesGraph.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class TransitionAttribute: Attribute
    {
        public Type TargetState { get; }

        public TransitionAttribute(Type target)
        {
            TargetState = target;
        }
    }
}
