using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Profiling.Rendering.Layout
{
    public sealed class ConsoleLayoutCalculator
    {
        public int MinDashboardHeight { get; set; } = 5;

        public int MinLogsHeight { get; set; } = 5;

        public double MaxDashboardRatio { get; set; } = 0.5;

        public ConsoleLayout Calculate(
            int terminalWidth,
            int terminalHeight,
            int dashboardContentHeight)
        {
            var availableHeight = Math.Max(0, terminalHeight);

            var maxDashboardHeight = (int)(
                availableHeight * MaxDashboardRatio);

            maxDashboardHeight = Math.Max(
                MinDashboardHeight,
                maxDashboardHeight);

            var maxDashboardByLogs = Math.Max(
                0,
                availableHeight - MinLogsHeight);

            maxDashboardHeight = Math.Min(
                maxDashboardHeight,
                maxDashboardByLogs);

            var dashboardHeight = Math.Clamp(
                dashboardContentHeight,
                Math.Min(MinDashboardHeight, maxDashboardHeight),
                maxDashboardHeight);

            var logsHeight = Math.Max(
                0,
                availableHeight - dashboardHeight);

            return new ConsoleLayout(
                terminalWidth,
                terminalHeight,
                dashboardHeight,
                logsHeight);
        }
    }
}
