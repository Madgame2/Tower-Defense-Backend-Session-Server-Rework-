using GameServer.Domain.Player;
using GameServer.Domain.SessionWorld.PlayerStorages.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.PlayerStorages
{
    internal class InMemmoryPlayerStorage : IPlayersStorage
    {
        private ConcurrentDictionary<string, Player.Player> _players = new();
        private volatile Player.Player[] _cachedPlayers = Array.Empty<Player.Player>();
        public void Add(Player.Player player)
        {
            if (_players.TryAdd(player.Id, player))
            {
                UpdateCache();
            }
        }

        public Player.Player? Get(string id)
        {
            return _players.TryGetValue(id, out var player) ? player : null;
        }

        public Player.Player[] GetAll() => _cachedPlayers;

        public void Remove(string id)
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
