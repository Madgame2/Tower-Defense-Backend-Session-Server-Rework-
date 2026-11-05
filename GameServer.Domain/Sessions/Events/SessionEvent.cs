using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.Sessions.Events
{
    public abstract record SessionEvent;

    public record PlayerConnected(string UserId)
    : SessionEvent;

    public record PlayerDisconnected(string UserId)
    : SessionEvent;
}
