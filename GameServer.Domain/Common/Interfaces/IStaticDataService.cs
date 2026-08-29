using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.Common.Interfaces
{
    public interface IStaticDataService
    {
        string GetText(string fileName);

        byte[] GetBytes(string fileName);
    }
}
