using GameServer.Profiling.Metrics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Profiling.Logging
{
    public static class DiagnosticsLoggingExtensions
    {
        public static ILoggingBuilder AddDiagnosticsConsole(
            this ILoggingBuilder builder,
            int logCapacity = 1000)
        {
            builder.Services.AddSingleton(
                new GameServer.Profiling.Storage.LogBuffer(logCapacity));

            builder.Services.AddSingleton<DiagnosticsLoggerProvider>();
            builder.Services.AddSingleton<TickMetricsCollector>();

            builder.Services.AddSingleton<ILoggerProvider>(
                sp => sp.GetRequiredService<DiagnosticsLoggerProvider>());

            return builder;
        }
    }
}
