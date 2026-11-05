using GameServer.Domain.Player;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.PlayerStorages.Interfaces
{
    public interface IPlayersStorage
    {
        Player.Player? Get(uint id);
        Player.Player? Get(string userId);
        Player.Player[] GetAll();
        void Add(Player.Player player);
        void Remove(uint id);
    }
}
