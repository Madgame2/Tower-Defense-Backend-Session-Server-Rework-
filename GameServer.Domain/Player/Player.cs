using GameServer.Domain.Player.Components;
using System;
using System.Collections.Generic;
using System.Net;
using System.Numerics;
using System.Text;

namespace GameServer.Domain.Player
{
    public class Player
    {
        public string Id { get; private set; }
        public Vector3 Position {  get; set; }
        public float Speed { get; private set; } = 1f;

        private readonly PlayerInputBuffer _playerInputBuffer = new();


        public PlayerInputBuffer InputBuffer {  get { return _playerInputBuffer; } }

        public Player(string id, Vector3 position)
        {
            Id = id;
            Position = position;
        }
    }
}
