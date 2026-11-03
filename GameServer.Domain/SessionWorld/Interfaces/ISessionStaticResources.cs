using GameServer.Domain.Core.GameObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces
{
    public interface ISessionStaticResources
    {
        GameObject? GetByName(string objectName);
    }
}
