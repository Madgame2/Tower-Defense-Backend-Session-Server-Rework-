using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;


namespace GameServer.Profiling.Logging
{
    public sealed class TDConsoleFormatter : ConsoleFormatter
    {
        public const string FormatterName = "td-diagnostics";

        private readonly IOptionsMonitor<ConsoleFormatterOptions> _options;

        public TDConsoleFormatter(
            IOptionsMonitor<ConsoleFormatterOptions> options)
            : base(FormatterName)
        {
            _options = options;
        }

        public override void Write<TState>(
    in LogEntry<TState> logEntry,
    IExternalScopeProvider? scopeProvider,
    TextWriter textWriter)
        {
            if (logEntry.LogLevel == LogLevel.None)
                return;

            var options = _options.CurrentValue;

            var message = logEntry.Formatter(
                logEntry.State,
                logEntry.Exception);

            // Timestamp
            if (!string.IsNullOrEmpty(options.TimestampFormat))
            {
                textWriter.Write(
                    DateTimeOffset.Now.ToString(
                        options.TimestampFormat,
                        CultureInfo.InvariantCulture));
            }

            // Log level
            var level = GetLogLevelName(logEntry.LogLevel);
            var color = GetLogLevelColor(logEntry.LogLevel);

            textWriter.Write(color);
            textWriter.Write(level);
            textWriter.Write("\u001b[0m");

            textWriter.Write(": ");
            textWriter.Write(logEntry.Category);
            textWriter.Write('[');
            textWriter.Write(logEntry.EventId.Id);
            textWriter.WriteLine("]");

            // Message
            textWriter.Write("      ");
            textWriter.Write(message);

            // Scopes
            if (options.IncludeScopes && scopeProvider != null)
            {
                scopeProvider.ForEachScope(
                    static (scope, writer) =>
                    {
                        writer.Write(" => ");
                        writer.Write(scope);
                    },
                    textWriter);
            }

            textWriter.WriteLine();

            // Exception
            if (logEntry.Exception != null)
            {
                textWriter.Write("      ");
                textWriter.WriteLine(logEntry.Exception);
            }
        }

        private static string GetLogLevelColor(LogLevel level)
        {
            return level switch
            {
                LogLevel.Trace => "\u001b[90m",
                LogLevel.Debug => "\u001b[37m",
                LogLevel.Information => "\u001b[32m",
                LogLevel.Warning => "\u001b[33m",
                LogLevel.Error => "\u001b[31m",
                LogLevel.Critical => "\u001b[97;41m",
                _ => "\u001b[0m"
            };
        }

        private static string GetLogLevelName(LogLevel level)
        {
            return level switch
            {
                LogLevel.Trace => "trce",
                LogLevel.Debug => "dbug",
                LogLevel.Information => "info",
                LogLevel.Warning => "warn",
                LogLevel.Error => "fail",
                LogLevel.Critical => "crit",
                _ => "none"
            };
        }
    }
}
