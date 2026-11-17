using GameServer.Domain.SessionWorld.Characters.model;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Domain.SessionWorld.Characters
{
    public interface ICharacterController
    {
        void ProceeLook(Vector3 lookPoint);
        void ProcessMove(Vector3 moveDirection);
        void ProcessRotation(Quaternion inputRotation);
        void ProcessJump();
        void ProcessAlternativeAction(PlayerActionState actionState, float delta);
        void ProcessPrimaryAction(PlayerActionState actionState, float delta);

    }
}
