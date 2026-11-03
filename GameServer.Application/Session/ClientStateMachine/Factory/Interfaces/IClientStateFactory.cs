using GameServer.Application.Session.ClientStateMachine.States.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Session.ClientStateMachine.Factory.Interfaces
{
    public interface IClientStateFactory
    {
        ClientStateBase Create(Type stateType);
    }
}
