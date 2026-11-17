using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.Core.GameObjects.Components
{
    public abstract class ComponentBase
    {
        public GameObject  GameObject{ get; set; }

        public virtual ComponentBase Clone()
        {
            throw new NotSupportedException(
                $"{GetType().Name} cannot be cloned.");
        }
    }
}
