using EntityLib.Infrastructure;
using EntityLib.Models;
using EntityLib.Services;
using GameServer.Domain.SessionWorld;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace EntityLib.GameRoomExtentions
{
    internal static class ExtentinonStoredData
    {
        public static readonly ConditionalWeakTable<GameRoom, ExternalData> _extData = new();

        public static ExternalData GetExternalData(GameRoom room)
        {
            return _extData.GetOrCreateValue(room);
        }
    }


    internal class ExternalData
    {
        public EntityManagerService EntityManager = new();
        public Dictionary<Type, IStash> StashesStorage = new();
    }
}
