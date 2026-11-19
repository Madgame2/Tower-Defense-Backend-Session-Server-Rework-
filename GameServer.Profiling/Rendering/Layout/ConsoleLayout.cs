using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Profiling.Rendering.Layout
{
    public readonly record struct ConsoleLayout(
        int TerminalWidth,
        int TerminalHeight,
        int DashboardHeight,
        int LogsHeight);
}
