using GameServer.Domain.ColliderSystem.Core;
using GameServer.Domain.Core.GameObjects;
using GameServer.Domain.Player.Components;
using GameServer.Domain.Player.Enums;
using System;
using System.Collections.Generic;
using System.Net;
using System.Numerics;
using System.Text;

namespace GameServer.Domain.Player
{
    public class Player : GameObject
    {
        public string Id { get; private set; }
        public Vector3 Velocity;
        public MovementState MovementState { get; set; } = MovementState.Grounded;
        public float Speed { get; private set; } = 2f;
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

        public Player(string id) : base("PlayerGameObject")
        {
            Id = id;
        }
    }
}
