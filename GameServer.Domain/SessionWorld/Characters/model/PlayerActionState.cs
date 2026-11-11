using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Characters.model
{
    public struct PlayerActionState
    {
        public bool IsPressed;
        public bool WasPressed;
        public bool WasReleased;
    }
}
