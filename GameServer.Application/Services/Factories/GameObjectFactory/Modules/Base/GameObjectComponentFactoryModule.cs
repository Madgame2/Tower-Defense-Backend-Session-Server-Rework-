using GameServer.Application.Services.Factories.GameObjectFactory.DTO;
using GameServer.Domain.Core.GameObjects.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace GameServer.Application.Services.Factories.GameObjectFactory.Modules.Base
{
    public abstract class GameObjectComponentFactoryModule
    {
        public abstract string Component { get; }

        public abstract ComponentBase CreateByXML(ComponentData data);
    }
}
