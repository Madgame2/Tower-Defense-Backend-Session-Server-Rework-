using GameServer.Domain.Player;
using GameServer.Domain.SessionWorld.PlayerStorages.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.PlayerStorages
{
    public class InMemmoryPlayerStorage : IPlayersStorage
    {
        private ConcurrentDictionary<uint, Player.Player> _players = new();
        private ConcurrentDictionary<string, Player.Player> _playersByClientID = new();
        private volatile Player.Player[] _cachedPlayers = Array.Empty<Player.Player>();
        public void Add(Player.Player player)
        {
            if (_players.TryAdd(player.PlayerId, player)&&
                _playersByClientID.TryAdd(player.UserId, player))
            {
                UpdateCache();
            }
        }

        public Player.Player? Get(uint id)
        {
            return _players.TryGetValue(id, out var player) ? player : null;
        }

        public Player.Player? Get(string userId)
        {
            return _playersByClientID.TryGetValue(userId, out var player) ? player : null;

        }

        public Player.Player[] GetAll() => _cachedPlayers;

        public void Remove(uint id)
        {
            if (_players.TryRemove(id, out _))
            {
                UpdateCache();
            }
        }
        private void UpdateCache()
        {
            _cachedPlayers = _players.Values.ToArray();
        }
    }
}
