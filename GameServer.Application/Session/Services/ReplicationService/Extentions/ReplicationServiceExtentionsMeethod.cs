using GameServer.Application.Session.Services.ReplicationService.Preprocessor;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.ReplicationLog.Preprocessor;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Session.Services.ReplicationService.Extentions
{
    public static class ReplicationServiceExtentionsMeethod
    {
        public static void AddReplicationService(this IServiceCollection services)
        {
            services.AddSingleton<ILogPreprocessor, LogPreprocessor>();
        }
    }
}
