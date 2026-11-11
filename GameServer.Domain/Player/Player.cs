using GameServer.Domain.ColliderSystem.Core;
using GameServer.Domain.Core.GameObjects;
using GameServer.Domain.Player.Components;
using GameServer.Domain.Player.Enums;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models;
using GameServer.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Net;
using System.Numerics;
using System.Text;

namespace GameServer.Domain.Player
{
    public class Player : GameObject
    {
        public uint PlayerId { get; private set; }
        public string UserId {  get; private set; }
        public MovementState MovementState { get; set; } = MovementState.Grounded;
        public ColliderShape Colider = new();
        public Vector3 Size {  get; set; }
        private Vector3 _pivot;
        public Vector3 Pivot { get =>_pivot; 
            set {
                _pivot = Vector3.Normalize(value);
            } }

        public bool IsGrounded = false;
        public bool IsJumping = false;

        private readonly PlayerInputBuffer _playerInputBuffer = new();

        public PlayerInputBuffer InputBuffer {  get { return _playerInputBuffer; } }

        private int _lastAcknowledgedTick;

        public uint LastAcknowledgedTick => unchecked((uint)Volatile.Read(ref _lastAcknowledgedTick));

        public Vector3 LookAt { get; set; }
        public CurrentClientActions CurrentStateSnapshot { get; set; }

        public void AcknowledgeTick(uint tick)
        {
            int newValue = unchecked((int)tick);

            while (true)
            {
                int current = Volatile.Read(ref _lastAcknowledgedTick);

                if (unchecked((uint)current) >= tick)
                    return;

                int previous = Interlocked.CompareExchange(
                    ref _lastAcknowledgedTick,
                    newValue,
                    current);

                if (previous == current)
                    return;
            }
        }

        public Player(uint playerId, string userId) : base("PlayerGameObject")
        {
            PlayerId = playerId;
            UserId = userId;
        }
    }
}
