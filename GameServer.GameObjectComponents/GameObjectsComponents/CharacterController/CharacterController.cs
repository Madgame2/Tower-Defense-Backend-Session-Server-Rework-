using GameServer.Domain.Core.GameObjects.Components;
using GameServer.Domain.SessionWorld.Characters;
using GameServer.Domain.SessionWorld.Characters.model;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.GameObjectComponentsAndServices.GameObjectsComponents.CharacterController
{
    public abstract class CharacterController : ComponentBase, ICharacterController, IReplicationModule<CharacterController>
    {
        public Type ProcessedType => typeof(CharacterController);

        event Action<ServerEvent> IReplicationModule.OnReplication
        {
            add => _onReplication += value;
            remove => _onReplication -= value;
        }

        public abstract void ProceeLook(Vector3 lookPoint);

        public abstract void ProcessAlternativeAction(PlayerActionState actionState, float delta);

        public abstract void ProcessJump();

        public abstract void ProcessMove(Vector3 moveDirection);

        public abstract void ProcessRotation(Quaternion inputRotation);


        protected Action<ServerEvent> _onReplication;
    }
}
