using GameServer.Application.Session.ClientStateMachine.Factory.Interfaces;
using GameServer.Application.Session.ClientStateMachine.States.Base;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Session.ClientStateMachine.Factory.Implementation
{
    public class ClientStateFactory : IClientStateFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ClientStateFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ClientStateBase Create(Type stateType)
        {
            if (!typeof(ClientStateBase).IsAssignableFrom(stateType))
            {
                throw new ArgumentException(
                    $"{stateType.Name} must inherit from ClientStateBase.",
                    nameof(stateType));
            }

            return (ClientStateBase)_serviceProvider.GetRequiredService(stateType);
        }
    }
}
