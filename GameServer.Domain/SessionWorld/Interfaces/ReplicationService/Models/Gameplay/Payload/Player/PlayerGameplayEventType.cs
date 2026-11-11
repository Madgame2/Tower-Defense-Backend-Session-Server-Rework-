using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Gameplay.Payload.Player
{
    public enum PlayerGameplayEventType : byte
    {
        InstantAction = 1,

        ContinuousActionStarted = 2,
        ContinuousActionEnded = 3,
        ContinuousActionUpdated = 4,
        ContinuousActionCancelled = 5
    }
}
