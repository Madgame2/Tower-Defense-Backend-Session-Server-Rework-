using GameServer.Profiling.Logging;
using GameServer.Profiling.Metrics;
using GameServer.Profiling.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Profiling.Rendering
{
    public sealed class ConsoleRenderer
    {
        private readonly LogBuffer _buffer;
        private readonly ProcessMetricsCollector _metricsCollector;
        private readonly TickMetricsCollector _tickMetrics;

        private readonly TimeSpan _refreshInterval =
            TimeSpan.FromMilliseconds(250);

        private readonly string[] _dashboardLines =
        {
        "TD SESSION SERVER",
        "----------------------------------------",
        "PERFORMANCE",
        "Network: Waiting for metrics",
        "----------------------------------------"
    };
        public ConsoleRenderer(
            LogBuffer buffer,
            TickMetricsCollector tickMetrics)
        {
            _buffer = buffer;
            _tickMetrics = tickMetrics;

            _metricsCollector = new ProcessMetricsCollector();
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            if (Console.IsOutputRedirected)
                throw new InvalidOperationException(
                    "Interactive rendering requires an interactive console.");

            var previousCursorVisibility = Console.CursorVisible;

            try
            {
                Console.CursorVisible = false;

                while (!cancellationToken.IsCancellationRequested)
                {
                    Render();

                    await Task.Delay(
                        _refreshInterval,
                        cancellationToken);
                }
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
            }
            finally
            {
                _metricsCollector.Dispose();

                Console.CursorVisible = previousCursorVisibility;
                Console.ResetColor();
            }
        }

        private string[] BuildDashboardLines()
        {
            var process = _metricsCollector.Collect();
            var tick = _tickMetrics.Collect();

            return new[]
            {
        "TD SESSION SERVER",
        "────────────────────────────────────────────────────────",
        "PROCESS",

        $"PID: {process.ProcessId,-12} " +
        $"Uptime: {process.Uptime.TotalHours:00}:{process.Uptime.Minutes:00}:{process.Uptime.Seconds:00} " +
        $"Threads: {process.ThreadCount}",

        $"CPU: {process.CpuUsage,5:F1}%       " +
        $"RAM: {FormatBytes(process.WorkingSet),-14} " +
        $"Private: {FormatBytes(process.PrivateMemory)}",

        $"GC Heap: {FormatBytes(process.ManagedMemory),-14} " +
        $"Allocated: {FormatBytes((long)process.AllocatedBytesPerSecond)}/s",

        $"GC: Gen0 {process.Gen0Collections} | " +
        $"Gen1 {process.Gen1Collections} | " +
        $"Gen2 {process.Gen2Collections}    " +
        $"Pause: {process.GcPausePercentage:F2}%",

        "────────────────────────────────────────────────────────",
        "SERVER",

        $"TPS: {tick.TicksPerSecond,5:F1} " +
        $"Avg: {tick.AverageDurationMs:F3} ms " +
        $"Max: {tick.MaxDurationMs:F3} ms",

        $"Allocated / Tick: {FormatBytes((long)tick.AllocatedBytesPerTick)} " +
        $"Max: {FormatBytes(tick.MaxAllocatedBytesPerTick)}",

        $"Tick Allocations: {FormatBytes(tick.AllocatedBytesPerSecond)}/s",

        "Sessions: 0       Players: 0       Status: Waiting..."
    };
        }

        private static string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };

            double value = bytes;
            var order = 0;

            while (value >= 1024 && order < sizes.Length - 1)
            {
                order++;
                value /= 1024;
            }

            return $"{value:F1} {sizes[order]}";
        }

        private void Render()
        {
            var width = Console.WindowWidth;
            var height = Console.WindowHeight;

            if (width <= 0 || height <= 0)
                return;

            var dashboardLines = BuildDashboardLines();

            var dashboardHeight = Math.Min(
                dashboardLines.Length,
                Math.Max(0, height - 5));

            var logsHeight = Math.Max(
                0,
                height - dashboardHeight);

            RenderDashboard(
                width,
                dashboardHeight,
                dashboardLines);

            RenderLogs(
                width,
                dashboardHeight,
                logsHeight);
        }

        private void RenderDashboard(
            int width,
            int height,
            string[] lines)
        {
            for (var i = 0; i < height; i++)
            {
                WriteLine(i, lines[i], width);
            }
        }

        private void RenderLogs(
            int width,
            int startRow,
            int availableHeight)
        {
            var entries = _buffer.GetSnapshot();

            var lines = new List<(string Text, LogLevel Level)>();

            foreach (var entry in entries)
            {
                var level = GetLevelName(entry.Level);

                var prefix = $"{level} ";

                var message = $"{entry.Category}: {entry.Message}";

                if (entry.Exception != null)
                    message += $" | {entry.Exception.Message}";

                message = message
                    .Replace('\r', ' ')
                    .Replace('\n', ' ');

                var availableWidth = Math.Max(1, width - prefix.Length);

                var wrappedLines = WrapText(message, availableWidth);

                for (var i = 0; i < wrappedLines.Count; i++)
                {
                    var line = i == 0
                        ? prefix + wrappedLines[i]
                        : new string(' ', prefix.Length) + wrappedLines[i];

                    lines.Add((line, entry.Level));
                }
            }

            var visibleCount = Math.Min(
                lines.Count,
                availableHeight);

            var startIndex = lines.Count - visibleCount;

            for (var i = 0; i < visibleCount; i++)
            {
                var line = lines[startIndex + i];

                WriteColoredLine(
                    startRow + i,
                    line.Text,
                    line.Level,
                    width);
            }

            for (var i = visibleCount; i < availableHeight; i++)
            {
                WriteLine(startRow + i, string.Empty, width);
            }
        }

        private static void WriteColoredLine(
            int row,
            string text,
            LogLevel level,
            int width)
        {
            Console.SetCursorPosition(0, row);
            Console.Write("\u001b[2K");

            if (text.Length > width)
                text = text[..width];

            var separatorIndex = text.IndexOf(' ');

            if (separatorIndex < 0)
            {
                Console.Write(GetLevelColor(level));
                Console.Write(text);
                Console.Write("\u001b[0m");
                return;
            }

            // Цветная метка уровня
            Console.Write(GetLevelColor(level));
            Console.Write(text[..separatorIndex]);
            Console.Write("\u001b[0m");

            // Обычный цвет сообщения
            Console.Write(text[separatorIndex..]);
        }

        private static List<string> WrapText(string text, int width)
        {
            var result = new List<string>();

            if (width <= 0)
                return result;

            if (string.IsNullOrEmpty(text))
            {
                result.Add(string.Empty);
                return result;
            }

            for (var i = 0; i < text.Length; i += width)
            {
                var length = Math.Min(width, text.Length - i);

                result.Add(text.Substring(i, length));
            }

            return result;
        }

        private static void WriteLog(
            int row,
            DiagnosticsLogEntry entry,
            int width)
        {
            Console.SetCursorPosition(0, row);

            Console.Write("\u001b[2K");

            Console.Write(
                GetLevelColor(entry.Level));

            var level = GetLevelName(entry.Level);

            Console.Write(level);

            Console.Write("\u001b[0m ");

            var message =
                $"{entry.Category}: {entry.Message}";

            if (entry.Exception != null)
                message += $" | {entry.Exception.Message}";

            message = message
                .Replace('\r', ' ')
                .Replace('\n', ' ');

            var remainingWidth = Math.Max(
                0,
                width - level.Length - 2);

            if (message.Length > remainingWidth)
                message = message[..remainingWidth];

            Console.Write(message);
        }

        private static void WriteLine(
            int row,
            string text,
            int width)
        {
            Console.SetCursorPosition(0, row);

            Console.Write("\u001b[2K");

            if (text.Length > width)
                text = text[..width];

            Console.Write(text);
        }

        private static string GetLevelName(LogLevel level)
        {
            return level switch
            {
                LogLevel.Trace => "TRCE",
                LogLevel.Debug => "DBUG",
                LogLevel.Information => "INFO",
                LogLevel.Warning => "WARN",
                LogLevel.Error => "FAIL",
                LogLevel.Critical => "CRIT",
                _ => "NONE"
            };
        }

        private static string GetLevelColor(LogLevel level)
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
    }
}
