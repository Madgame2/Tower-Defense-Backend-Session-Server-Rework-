using GameServer.Application.Session.ClientStateMachine.Factory.Implementation;
using GameServer.Application.Session.ClientStateMachine.Factory.Interfaces;
using GameServer.Application.Session.ClientStateMachine.States;
using GameServer.Application.Session.ClientStateMachine.States.Base;
using GameServer.Application.Session.PlayersStateMachine.Implementation;
using GameServer.Domain.Services.ClientStateMachine;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Session.ClientStateMachine.Extentions
{
    public static class CleintStatemachineExtentionMethods
    {
        public static void AddClentStateMachine(this IServiceCollection services)
        {

            services.AddTransient<IInitialClientState, ClientConnectedState>();

            services.AddTransient<ClientRegisterState>();
            services.AddTransient<ClientSyncState>();
            services.AddTransient<ClientReadyState>();

            services.AddSingleton<IClientStateFactory, ClientStateFactory>();
            services.AddTransient<IClientStateMachine, ClientsStateMachine>();
        }
    }
}
