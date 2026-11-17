using GameServer.Domain.ValueObjects.Enums;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Domain.ValueObjects
{
    public struct CurrentClientActions
    {
        public Vector3 MoveDirection;
        public Quaternion Rotation;
        public Vector3 LookAt;
        public ActionFlags Actions;

        public CurrentClientActions(PlayerStateSnapShootClient playerInput)
        {
            this.MoveDirection = playerInput.MoveDirection;
            this.Rotation = playerInput.Rotation;
            this.LookAt = playerInput.LookAtPoint;
            this.Actions = playerInput.Actions;
        }

        public bool JumpFlag { get => (Actions & ActionFlags.JumpRequested) != 0; }
        public bool AlternativeActonFlag { get => (Actions & ActionFlags.AlternativeAction) != 0; }
        public bool PrimaryActionFlag { get => (Actions & ActionFlags.PrimaryAction) != 0; }
    }
}
