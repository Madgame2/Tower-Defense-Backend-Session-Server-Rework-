using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GameServer.SearchComponents.AdaptersRegistry.Index.Model
{
    public sealed record QueryCapabilityAssembly(Assembly Assembly);
    public sealed record FullScanCapabilityAssembly(Assembly Assembly);
}
