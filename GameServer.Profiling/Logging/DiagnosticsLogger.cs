using GameServer.Profiling.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Profiling.Logging
{
    internal sealed class DiagnosticsLogger : ILogger
    {
        private readonly string _category;
        private readonly LogBuffer _buffer;

        public DiagnosticsLogger(
            string category,
            LogBuffer buffer)
        {
            _category = category;
            _buffer = buffer;
        }

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            ArgumentNullException.ThrowIfNull(formatter);

            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message) && exception == null)
                return;

            var entry = new DiagnosticsLogEntry(
                DateTimeOffset.Now,
                logLevel,
                _category,
                eventId,
                message,
                exception);

            _buffer.Enqueue(entry);
        }
    }
}
