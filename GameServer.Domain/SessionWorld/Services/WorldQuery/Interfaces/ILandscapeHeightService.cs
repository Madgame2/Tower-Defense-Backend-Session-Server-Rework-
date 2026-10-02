using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.Interfaces
{
    public interface ILandscapeHeightService
    {
        Task<float> GetHeightAt(float worldX, float worldZ);        
    }
}
